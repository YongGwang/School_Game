
#include <stdio.h>
#include <windows.h>

typedef enum _JOB
{
	Assassin,
	Knight,
	Doctor,
	Ninja,
	Monstertamer,
	None
}JOB;

typedef struct _ACOUNT
{
	char strName[20];
	JOB eJob;
	int nStrenth;
	int nAgility;
	int nWisdom;
	int nLuck;
	int nCombatPower;
}ACOUNT;

typedef struct _DATABASE
{
	int nAcountCount;
	ACOUNT* pAcount;
}DATABASE;

DATABASE * g_pDatabase = NULL;
int g_nDataTotal = 0;

void gotoxy(int x, int y)
{
	COORD CursorPosition = { x, y };
	SetConsoleCursorPosition(GetStdHandle(STD_OUTPUT_HANDLE), CursorPosition);
}

void ExceptHandleNum(int* p_Num)
{
	if (*p_Num > 3000)
	{
		*p_Num = 3000;
	}
	else if (*p_Num < 0)
	{
		*p_Num = 0;
	}
}

void ExceptHandleMemNum(int* p_Num)
{
	if (*p_Num > 300)
	{
		*p_Num = 300;
	}
	else if (*p_Num < 0)
	{
		*p_Num = 0;
	}
}

void Destroy()
{
	int i;

	//全体データを初期化するところ
	for (i = 0; i < g_nDataTotal; ++i)
	{
		if (g_pDatabase[i].pAcount != NULL)
		{
			free(g_pDatabase[i].pAcount);
		}
	}
}

void Init()
{
	int i, j;
	Destroy();

	system("cls");
	//けいせん変換
	printf("┌──────────────────────────────────────────────────────────┐\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　生成しようとしているデータベースの数を入力してください　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　生成するデータベースの数：　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("└──────────────────────────────────────────────────────────┘\n");

	gotoxy(45, 5);
	scanf("%d", &g_nDataTotal);
	rewind(stdin);
	//memory例外処理は300まで
	ExceptHandleMemNum(&g_nDataTotal);
	g_pDatabase = (ACOUNT*)malloc(sizeof(ACOUNT)*g_nDataTotal);

	system("cls");
	printf("┌──────────────────────────────────────────────────────────┐\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　生成しようとしているID数を入力してください　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("└──────────────────────────────────────────────────────────┘\n");

	//データベースの数に当たるメモリー生成
	for (i = 0; i < g_nDataTotal; ++i)
	{
		gotoxy(4, 5);
		printf("　　　　　　　　　　　　　　　　　　　　　　　　　　　"); //一桁を消す
		gotoxy(6, 5);
		printf("データベース番号　%d　　アカウントの数： ", i + 1);
		scanf("%d", &g_pDatabase[i].nAcountCount);
		rewind(stdin);
		ExceptHandleMemNum(&g_pDatabase[i].nAcountCount);
		g_pDatabase[i].pAcount = (ACOUNT*)malloc(sizeof(ACOUNT) * g_pDatabase[i].nAcountCount);

		for (j = 0; j < g_pDatabase[i].nAcountCount; ++j)
		{
			strcpy(g_pDatabase[i].pAcount[j].strName, "None");
			g_pDatabase[i].pAcount[j].eJob = (JOB)5;
			g_pDatabase[i].pAcount[j].nStrenth = 0;
			g_pDatabase[i].pAcount[j].nAgility = 0;
			g_pDatabase[i].pAcount[j].nWisdom = 0;
			g_pDatabase[i].pAcount[j].nLuck = 0;
			g_pDatabase[i].pAcount[j].nCombatPower = 0;
		}
	}
}

int MainMenu()
{
	int nKey;

	printf("┌──────────────────────────────────────────────────────────┐\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　┃　アカウント管理処理プログラム　┃　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　０．初期化　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　１．入力　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　２．修正　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　３．検索　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　４．出力　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　５．終了　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　メニューを選択してください　（　　　　　　）　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("└──────────────────────────────────────────────────────────┘\n");

	gotoxy(46, 16);
	scanf("%d", &nKey);
	return nKey;
}

void AcountInfoDisplay(int nIndex)
{
	char* strTitle[4] = { "┃　データ入力　┃", "┃　データ修正　┃", "┃　データ検索　┃" };
	system("cls");
	printf("┌──────────────────────────────────────────────────────────┐\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　データベース：　　　　　　　　アカウント番号：　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　職業番号　　　　　　　　　　　　│\n");
	printf("│０．Assassin１．Knight２．Doctor３．Ninja４．Monstertamer │\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　名前：　　　　　　　　　職業：　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　◆能力値　　能力値は3000まで入力できます　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　ストレングス　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　アジリティー　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　知恵　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　運　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("└──────────────────────────────────────────────────────────┘\n");
	gotoxy(20, 1);
	printf("%s", strTitle[nIndex]);
}

void Input()
{
	char chYESorNO;
	ACOUNT sAcount;
	int nDatabase, nNum;
	int jobTemp;

	//間違って入力されたときメニューに戻る
	while (1)
	{
		system("cls");
		AcountInfoDisplay(0);

		//データベース番号は1番から
		gotoxy(20, 4);
		scanf("%d", &nDatabase);
		//メモリー生成したデータベース以外の番号、文字が入力されると
		rewind(stdin);
		if (nDatabase > g_nDataTotal || 0 > nDatabase)
			return;

		gotoxy(3, 3);
		printf("◆%d　データベースの総入力可能なアカウントは%dです。", nDatabase, g_pDatabase[nDatabase - 1].nAcountCount); //追加
		gotoxy(52, 4);
		scanf("%d", &nNum);
		rewind(stdin);
		if (nNum > g_pDatabase[nDatabase -1].nAcountCount || 0 > nNum)
			return;

		gotoxy(13, 9);
		scanf("%s", sAcount.strName);
		rewind(stdin);

		//０～4まで
		//Assassin,Knight,Doctor,Ninja,Monstertamer
		gotoxy(36, 9);
		scanf("%d", &jobTemp);
		if (jobTemp < 0 || jobTemp >4)	jobTemp = 0;
		sAcount.eJob = (JOB)jobTemp;
		rewind(stdin);

		//能力値は3000まで入力できる。
		gotoxy(20, 13);
		scanf("%d", &sAcount.nStrenth);
		rewind(stdin);
		ExceptHandleNum(&sAcount.nStrenth);

		gotoxy(20, 15);
		scanf("%d", &sAcount.nAgility);
		rewind(stdin);
		ExceptHandleNum(&sAcount.nAgility);

		gotoxy(20, 17);
		scanf("%d", &sAcount.nWisdom);
		rewind(stdin);
		ExceptHandleNum(&sAcount.nWisdom);

		gotoxy(20, 19);
		scanf("%d", &sAcount.nLuck);
		rewind(stdin);
		ExceptHandleNum(&sAcount.nLuck);

		sAcount.nCombatPower = sAcount.nStrenth + sAcount.nAgility + sAcount.nWisdom + sAcount.nLuck;

		gotoxy(22, 22);
		printf("総合戦闘力 : %d", sAcount.nCombatPower);

		gotoxy(16, 24);
		printf("保存しますか？Y/N?[      ]");
		gotoxy(37, 24);
		scanf("%c", &chYESorNO);
		rewind(stdin);
		
		if (chYESorNO == 'n')
		{
			return ;
		}
		else
		{
			g_pDatabase[nDatabase - 1].pAcount[nNum - 1] = sAcount; //インデックスは０から始める

			while (1)
			{
				gotoxy(16, 26);
				printf("続きますか？　Y/N?[      ]");
				gotoxy(37, 26);
				scanf("%c", &chYESorNO);
				rewind(stdin);
				if (chYESorNO == 'n')
				{
					return;
				}
				else
				{
					break;
				}
			}
		}
	}
}

void SearchDisplay(int *pDatabase, int *pNum)
{
	system("cls");
	printf("┌──────────────────────────────────────────────────────────┐\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　┃　データ検索　┃　　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("│　データベース:　　　　　 アカウント番号:　　　　　　　　　│\n");
	printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
	printf("└──────────────────────────────────────────────────────────┘\n");

	do
	{
		gotoxy(18, 4);
		printf("          ");
		gotoxy(18, 4);
		scanf("%d", pDatabase);
		rewind(stdin);
	} while (*pDatabase > g_nDataTotal);

	gotoxy(44, 4);
	scanf("%d", pNum);
	rewind(stdin);
}

void Modify()
{
	char chYESorNO;
	ACOUNT sAcount;
	char strData[20];
	int nTemp, nAcount, nNum;

	while (1)
	{
		SearchDisplay(&nAcount, &nNum);
		sAcount = g_pDatabase[nAcount - 1].pAcount[nNum - 1];
		AcountInfoDisplay(1);

		gotoxy(20, 4);
		printf("%-3d", nAcount);

		gotoxy(52, 4);
		printf("%-3d", nNum);

		gotoxy(13, 9);
		printf("%s", sAcount.strName);

		gotoxy(36, 9);
		printf("%-2d", sAcount.eJob);

		gotoxy(20, 13);
		printf("%-2d", sAcount.nStrenth);

		gotoxy(20, 15);
		printf("%-2d", sAcount.nAgility);

		gotoxy(20, 17);
		printf("%-2d", sAcount.nWisdom);

		gotoxy(20, 19);
		printf("%-2d", sAcount.nLuck);

		gotoxy(22, 22);
		printf("総合戦闘力 : %d", sAcount.nCombatPower);

		//修正項目
		gotoxy(13, 10);
		gets(strData);
		if (strlen(strData) > 0)
		{
			strcpy(sAcount.strName, strData);
		}

		gotoxy(36, 10);
		gets(strData);
		if (strlen(strData) > 0)
		{
			nTemp = atoi(strData);
			sAcount.eJob = nTemp;
		}

		gotoxy(20, 14);
		gets(strData);
		if (strlen(strData) > 0)
		{
			nTemp = atoi(strData);
			sAcount.nStrenth = nTemp;
		}

		gotoxy(20, 16);
		gets(strData);
		if (strlen(strData) > 0)
		{
			nTemp = atoi(strData);
			sAcount.nAgility = nTemp;
		}

		gotoxy(20, 18);
		gets(strData);
		if (strlen(strData) > 0)
		{
			nTemp = atoi(strData);
			sAcount.nWisdom = nTemp;
		}

		gotoxy(20, 20);
		gets(strData);
		if (strlen(strData) > 0)
		{
			nTemp = atoi(strData);
			sAcount.nLuck = nTemp;
		}

		sAcount.nCombatPower = sAcount.nStrenth + sAcount.nAgility + sAcount.nWisdom + sAcount.nLuck;
		gotoxy(36, 23);
		printf("%-5d", sAcount.nCombatPower);

		gotoxy(16, 24);
		printf("保存しますか？Y/N?[      ]");
		gotoxy(37, 24);
		scanf("%c", &chYESorNO);
		rewind(stdin);

		if (chYESorNO == 'n')
		{
			return;
		}
		else
		{
			g_pDatabase[nAcount - 1].pAcount[nNum - 1] = sAcount;

			while (1)
			{
				gotoxy(16, 26);
				printf("続きますか？　Y/N?[      ]");
				gotoxy(37, 26);
				scanf("%c", &chYESorNO);
				rewind(stdin);
				if (chYESorNO == 'n')
				{
					return;
				}
				else
				{
					break;
				}
			}
		}
	}
}

void Search()
{
	char chYESorNO;
	int nDatabase, nNum;
	while (1)
	{
		SearchDisplay(&nDatabase, &nNum);
		AcountInfoDisplay(2);

		gotoxy(20, 4);
		printf("%d", nDatabase);

		gotoxy(52, 4);
		printf("%4d", nNum);

		gotoxy(13, 9);
		printf("%s", g_pDatabase[nDatabase - 1].pAcount[nNum - 1].strName);

		gotoxy(36, 9);
		printf("%d", g_pDatabase[nDatabase - 1].pAcount[nNum - 1].eJob);

		gotoxy(20, 13);
		printf("%d", g_pDatabase[nDatabase - 1].pAcount[nNum - 1].nStrenth);

		gotoxy(20, 15);
		printf("%d", g_pDatabase[nDatabase - 1].pAcount[nNum - 1].nAgility);

		gotoxy(20, 17);
		printf("%d", g_pDatabase[nDatabase - 1].pAcount[nNum - 1].nWisdom);

		gotoxy(20, 19);
		printf("%d", g_pDatabase[nDatabase - 1].pAcount[nNum - 1].nLuck);

		gotoxy(22, 22);
		printf("総合戦闘力 : %d", g_pDatabase[nDatabase - 1].pAcount[nNum - 1].nCombatPower);

		while (1)
		{
			gotoxy(20, 30);
			printf("続きますか？ Y/N[　　　　　　]");
			gotoxy(50, 30);
			scanf("%c", &chYESorNO);
			rewind(stdin);

			if (chYESorNO == 'n')
			{
				return;
			}
			else
			{
				break;
			}
		}
	}
}

void Print()
{
	int i, nDatabase;
	char chYESorNO;
	int nCursorY = 0;
	while (1)
	{
		system("cls");
		printf("┌──────────────────────────────────────────────────────────┐\n");
		printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
		printf("│　　　　　　　　　　出力：データ検索　　　　　　　　　　　│\n");
		printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
		printf("│　　データベース番号：　　　　　　　　　　　　　　　　　　│\n");
		printf("│　　　　　　　　　　　　　　　　　　　　　　　　　　　　　│\n");
		printf("└──────────────────────────────────────────────────────────┘\n");

		gotoxy(25, 4);
		scanf("%d", &nDatabase);
		rewind(stdin);

		system("cls");
		printf("　　　　　　　　　　　　　　　　　　　　　　┃　%d のデータベース　┃\n", nDatabase);
		printf("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");
		printf("アカウント番号　　　　　名前　　　　職業　　ストレングス　　アジリティー　知恵　　　運　　　総合戦闘力");
		printf("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");
		if (g_pDatabase == NULL)
		{
			printf("%d　のデータベースが生成されてないです。", nDatabase);
		}
		else
		{
			for (i = 0; i < g_pDatabase[nDatabase - 1].nAcountCount; ++i)
			{
				if ((g_pDatabase + i) != NULL)
				{
					printf("　　%2d　 %20s　　　　%d　　　　%4d　　　　　　%4d　　　　%4d　　　%4d　　　%6d\n", i + 1,
						g_pDatabase[nDatabase - 1].pAcount[i].strName, g_pDatabase[nDatabase - 1].pAcount[i].eJob,
						g_pDatabase[nDatabase - 1].pAcount[i].nStrenth, g_pDatabase[nDatabase - 1].pAcount[i].nAgility,
						g_pDatabase[nDatabase - 1].pAcount[i].nWisdom, g_pDatabase[nDatabase - 1].pAcount[i].nLuck,
						g_pDatabase[nDatabase - 1].pAcount[i].nCombatPower);
				}
				nCursorY++;
			}
		}
		printf("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

		while (1)
		{
			printf("　　　　　　　　　　　　　　続きますか？ Y/N[　　　　　　]");
			gotoxy(50, nCursorY + 7);
			scanf("%c", &chYESorNO);
			rewind(stdin);
			nCursorY = 0;

			if (chYESorNO == 'n')
			{
				return;
			}
			else
			{
				break;
			}
		}
	}
}

int main(void)
{
	int nKey;

	while (1)
	{
		system("cls");
		nKey = MainMenu();

		if (nKey == 5)
			break;

		switch (nKey)
		{
		case 0:	//初期化
			Init();
			break;
		case 1:	//入力
			Input();
			break;
		case 2:	//修正
			Modify();
			break;
		case 3:	//検索
			Search();
			break;
		case 4:	//出力
			Print();
			break;
		}
	}

	Destroy();

	system("cls");
	gotoxy(20, 10);
	printf("Good Bye!\n");

	_getch();
	return 0;
}