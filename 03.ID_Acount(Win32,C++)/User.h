#pragma once
#include <list>
using namespace std;

struct INFO_USER
{
	char sNum[9];	// アカウント生成年度 XX, 職業コード XX, 個人番号 XXXX  8桁
	char sName[20];		
	char sCountry[30];
	char sPhoneNum[15];
	char sEmail[30];

	int nPoints[2][2]; //シーズン点数
};

class CUser
{
	list<INFO_USER> mUserList;

public:
	bool Save();
	bool Load();
	int Find(char* pNumber, INFO_USER* pStudentInfo);	//IDで検索
	int Find(char* pNumber);
	bool SetScore(int nIndex, int First, int Late, int nScore);	//ID、シーズン１、シーズン２、点数
	bool AddUser(INFO_USER sStudentInfo);
	bool DeleteUser(int nIndex);
	bool SetUserInfo(int nIndex, INFO_USER sStudentInfo);
	void SetPointClear(int nIndex);
public:
	CUser(void);
	~CUser(void);
};
