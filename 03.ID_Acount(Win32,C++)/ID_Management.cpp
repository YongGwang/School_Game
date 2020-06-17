//アカウント管理ツール
//ハンガリアン表記法

#include "framework.h"
#include "ID_Management.h"
#include <WindowsX.h>
#include <iostream>
#include <vector>
#include <CommCtrl.h>
#include <stdio.h>      //ファイル入力、出力するため
#include "User.h"
using namespace std;

static HDC hMemDC;

//プロシージャ
INT_PTR CALLBACK DlgProcr_Login(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK DlgProcr_Menu(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK DlgProcr_IDINFO(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK DlgProcr_USERINFO(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK DlgProcr_USERList(HWND, UINT, WPARAM, LPARAM);

void UserInfoClear(HWND hDlg);

//ログイン
struct INFO_LOGIN
{
    char id[20];
    char password[20];
};

HINSTANCE hInst;            //現在のインターフェイス
INFO_LOGIN gsLoginInfo;     //管理者ID
HWND gMainWnd;
CUser cUser;

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPWSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
    hInst = hInstance;
    if (DialogBox(hInstance, MAKEINTRESOURCE(IDD_LOGIN), 0, DlgProcr_Login) == -1)
        //Windowプロシージャが終わる
        return 0;
    //MAINフォームを呼び出し
    DialogBox(hInstance, MAKEINTRESOURCE(IDD_BACKWINDOW), 0, DlgProcr_Menu);
    return 0;
}

//ログイン判定
INT_PTR CALLBACK DlgProcr_Login(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    static INFO_LOGIN sLoginInfo;
    static HWND hIDWnd, hPasswordWnd;
    INFO_LOGIN sInputLogin;
    FILE* folderdirection;
    UNREFERENCED_PARAMETER(lParam);

    switch (message)
    {
    case WM_INITDIALOG:
        //ファイルの名前とどう使えるか指定
        folderdirection = fopen(("pw.txt"), ("r"));
        fscanf(folderdirection, "%s %s", sLoginInfo.id, sLoginInfo.password);   //初期ID,PW:SongSong, 19880111
        fclose(folderdirection);
        return (INT_PTR)TRUE;
    case WM_CLOSE:
        EndDialog(hDlg, 0);
        DeleteDC(hMemDC);
        PostQuitMessage(0);
        return (INT_PTR)TRUE;

    case WM_COMMAND:
        switch (LOWORD(wParam))
        {
        case IDOK:
            GetDlgItemText(hDlg, IDC_EDIT1, sInputLogin.id, 12);
            GetDlgItemText(hDlg, IDC_EDIT2, sInputLogin.password, 12);

            if (lstrcmp(sLoginInfo.id, sInputLogin.id) != 0)
            {
                MessageBox(0, TEXT("IDが一致しません。"), TEXT("エラー"), MB_OK);
                SetDlgItemText(hDlg, IDC_EDIT1, TEXT(""));
                SetFocus(GetDlgItem(hDlg, IDC_EDIT1));
                return (INT_PTR)TRUE;   //EndDialog()を呼び出さなかったのでDialogは出力された状態
            }

            if (lstrcmp(sLoginInfo.password, sInputLogin.password) != 0)
            {
                MessageBox(0, TEXT("PWが一致しません。"), TEXT("エラー"), MB_OK);
                SetDlgItemText(hDlg, IDC_EDIT2, TEXT(""));
                SetFocus(GetDlgItem(hDlg, IDC_EDIT2));
                return (INT_PTR)TRUE;   //EndDialog()を呼び出さなかったのでDialogは出力された状態
            }
            memcpy(&gsLoginInfo, &sLoginInfo, sizeof(INFO_LOGIN));
            EndDialog(hDlg, 1);
            return (INT_PTR)TRUE;
        case IDCANCEL:
            EndDialog(hDlg, -1);
            DeleteDC(hMemDC);
            PostQuitMessage(0);
            return (INT_PTR)TRUE;
        }
    }
    return (INT_PTR)FALSE;
}


INT_PTR CALLBACK DlgProcr_Menu(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);
    char str[100];
    switch (message)
    {
    case WM_INITDIALOG:
        gMainWnd = hDlg;
        sprintf(str, "データベース管理プログラムです。ログインしているIDは%sです。[Ver0.1]", TEXT(gsLoginInfo.id));
        SetWindowText(hDlg, str);
        return (INT_PTR)TRUE;
    case WM_CLOSE:
        EndDialog(hDlg, LOWORD(wParam));
        return (INT_PTR)TRUE;
    case WM_COMMAND:
        switch (LOWORD(wParam))
        {
        case ID_EXIT:
            EndDialog(hDlg, LOWORD(wParam));
            DeleteDC(hMemDC);
            PostQuitMessage(0);
            return (INT_PTR)TRUE;
        case ID_IDINFO: //ID情報
            DialogBox(hInst, MAKEINTRESOURCE(IDD_INFO_CHANGE), hDlg, DlgProcr_IDINFO);
            sprintf(str, "今ログインしているIDは　%s　です。　「ver 0.0」", gsLoginInfo.id);
            SetWindowText(gMainWnd, str);
            return (INT_PTR)true;
        case ID_USERINFO: //ユーザー情報
            DialogBox(hInst, MAKEINTRESOURCE(IDD_USER_INFO), hDlg, DlgProcr_USERINFO);
            return (INT_PTR)true;
        //case ID_USERRANK: //情報検索　作る予定
        }
        break;
    }
    return (INT_PTR)FALSE;
}

//管理者IDとパスワード変更処理
INT_PTR CALLBACK DlgProcr_IDINFO(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    INFO_LOGIN sLoginInfo;
    char str[100];
    FILE* folderdirection;
    UNREFERENCED_PARAMETER(lParam);
    switch (message)
    {
    case WM_INITDIALOG:
        SetDlgItemText(hDlg, IDC_EDIT1, gsLoginInfo.id);
        return (INT_PTR)TRUE;

    case WM_COMMAND:
        switch (LOWORD(wParam))
        {
        case IDOK:
            // 変更情報確認
            GetDlgItemText(hDlg, IDC_EDIT2, sLoginInfo.id, 12);         //ID
            GetDlgItemText(hDlg, IDC_EDIT3, sLoginInfo.password, 12);   //Password
            GetDlgItemText(hDlg, IDC_EDIT4, str, 14);                //Password確認
            if (strcmp(sLoginInfo.password, str) != 0)
            {
                MessageBox(0, "パスワードが違います。", "エラー", MB_OK);
                SetDlgItemText(hDlg, IDC_EDIT3, "");
                SetDlgItemText(hDlg, IDC_EDIT4, "");
                SetFocus(GetDlgItem(hDlg, IDC_EDIT3));
                return (INT_PTR)TRUE;
            }

            //両方一致した場合の処理
            folderdirection = fopen("pw.txt", "w");
            fprintf(folderdirection, "%s %s", sLoginInfo.id, sLoginInfo.password);
            fclose(folderdirection);
            memcpy(&gsLoginInfo, &sLoginInfo, sizeof(INFO_LOGIN));
            EndDialog(hDlg, 0);
            return (INT_PTR)TRUE;
        case IDCANCEL:
            EndDialog(hDlg, LOWORD(wParam));
            return (INT_PTR)TRUE;
        }
        break;
    }
    return (INT_PTR)FALSE;
}

//ユーザー情報管理プロシージャ
INT_PTR CALLBACK DlgProcr_USERINFO(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);
    char* strJobName[] = { "パラディン", "アサシン", "サイコパス", "クレリック" }; //ex)XX00XXXX  ２，３のインデックスによって職業が決まる
    char str[100];
    INFO_USER sUserInfo;
    int nIndex;
    //サイコパスまで職業出力
    char strStatCode[3] = { 0, };

    switch (message)
    {
    case WM_INITDIALOG:
        if (cUser.Load() == FALSE)
            MessageBox(0, "ファイルがないですよ。", "メッセージ", MB_OK);
        return (INT_PTR)TRUE;
    case WM_CLOSE:
        EndDialog(hDlg, LOWORD(wParam));
        return (INT_PTR)TRUE;

    case WM_COMMAND:
        switch (LOWORD(wParam))
        {
        case IDC_BUTTON1: // 検索
            GetDlgItemText(hDlg, IDC_EDIT1, str, 8); // ID
            nIndex = cUser.Find(str, &sUserInfo);
            if (nIndex != -1)
            {
                strStatCode[0] = str[2]; 
                strStatCode[1] = str[3];
                
                SetDlgItemText(hDlg, IDC_EDIT2, sUserInfo.sName); //名前
                SetDlgItemText(hDlg, IDC_EDIT3, sUserInfo.sCountry); //国籍
                SetDlgItemText(hDlg, IDC_EDIT4, sUserInfo.sPhoneNum); //携帯電話
                SetDlgItemText(hDlg, IDC_EDIT5, sUserInfo.sEmail); //Email
                SetDlgItemText(hDlg, IDC_EDIT6, strJobName[atoi(strStatCode)]); //職業をインデックスに使う(char to int)　ex)XX00XXXX 00を使って職業出力
            }
            else {
                MessageBox(0, "ユーザーの情報がないです。", "ユーザー情報", MB_OK);
                UserInfoClear(hDlg);
                SetDlgItemText(hDlg, IDC_EDIT1, str); //検索した番号はそのまま置く
            }
            return (INT_PTR)TRUE;

        case IDC_BUTTON2: //追加
            GetDlgItemText(hDlg, IDC_EDIT1, sUserInfo.sNum, 8); //IDインデックス
            GetDlgItemText(hDlg, IDC_EDIT2, sUserInfo.sName, 20); //名前
            GetDlgItemText(hDlg, IDC_EDIT3, sUserInfo.sCountry, 200); //国籍
            GetDlgItemText(hDlg, IDC_EDIT4, sUserInfo.sPhoneNum, 14); //携帯電話
            GetDlgItemText(hDlg, IDC_EDIT5, sUserInfo.sEmail, 30); //Email
            //https://stackoverflow.com/questions/7367677/is-memset-more-efficient-than-for-loop-in-c
            //forの代わりにTest
            memset(sUserInfo.nPoints, -1, sizeof(sUserInfo.nPoints)); // -1で初期化
            cUser.AddUser(sUserInfo);
            MessageBox(0, "入力完了", "完了", MB_OK);
            UserInfoClear(hDlg);
            return (INT_PTR)TRUE;
        case IDC_BUTTON3: // 修正ボタン
            GetDlgItemText(hDlg, IDC_EDIT1, str, 8); // ID
            nIndex = cUser.Find(str, &sUserInfo);
            if (nIndex != -1)
            {
                GetDlgItemText(hDlg, IDC_EDIT1, sUserInfo.sNum, 8);
                GetDlgItemText(hDlg, IDC_EDIT3, sUserInfo.sName, 19);
                GetDlgItemText(hDlg, IDC_EDIT4, sUserInfo.sCountry, 200);
                GetDlgItemText(hDlg, IDC_EDIT5, sUserInfo.sPhoneNum, 14);
                GetDlgItemText(hDlg, IDC_EDIT6, sUserInfo.sEmail, 29);
                cUser.SetUserInfo(nIndex, sUserInfo);

                MessageBox(0, "!", "ユーザー情報", MB_OK);
            }
            else {
                MessageBox(0, "修正するユーザー情報がないですよ。", "ユーザー情報", MB_OK);
                UserInfoClear(hDlg);
            }
            return (INT_PTR)TRUE;
        case IDC_BUTTON4: // 削除、ID情報と中身全部削除
            GetDlgItemText(hDlg, IDC_EDIT1, str, 8); // IDで検索して呼び出し
            nIndex = cUser.Find(str);
            if (nIndex != -1)
            {
                cUser.DeleteUser(nIndex);
            }
            MessageBox(0, "削除しました。", "完了", MB_OK);
            return (INT_PTR)TRUE;
        case IDC_BUTTON5: // 終了
            UserInfoClear(hDlg);
            return (INT_PTR)TRUE;
        case IDC_BUTTON6: // 初期化
            EndDialog(hDlg, LOWORD(wParam));
            return (INT_PTR)TRUE;
        }
        break;
    }
    return (INT_PTR)FALSE;
}

INT_PTR CALLBACK DlgProcr_USERList(HWND, UINT, WPARAM, LPARAM)
{
    return (INT_PTR)FALSE;
}


void UserInfoClear(HWND hDlg)
{
    SetDlgItemText(hDlg, IDC_EDIT1, ""); //IDインデックス
    SetDlgItemText(hDlg, IDC_EDIT2, ""); //名前
    SetDlgItemText(hDlg, IDC_EDIT3, ""); //国籍
    SetDlgItemText(hDlg, IDC_EDIT4, ""); //携帯電話
    SetDlgItemText(hDlg, IDC_EDIT5, ""); //Email
    SetDlgItemText(hDlg, IDC_EDIT6, ""); //ID番号をインデックスに使う
}