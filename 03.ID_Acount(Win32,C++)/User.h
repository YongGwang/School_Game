#pragma once
#include <list>
using namespace std;

struct INFO_USER
{
	char sNum[9];	// �A�J�E���g�����N�x XX, �E�ƃR�[�h XX, �l�ԍ� XXXX  8��
	char sName[20];		
	char sCountry[30];
	char sPhoneNum[15];
	char sEmail[30];

	int nPoints[2][2]; //�V�[�Y���_��
};

class CUser
{
	list<INFO_USER> mUserList;

public:
	bool Save();
	bool Load();
	int Find(char* pNumber, INFO_USER* pStudentInfo);	//ID�Ō���
	int Find(char* pNumber);
	bool SetScore(int nIndex, int First, int Late, int nScore);	//ID�A�V�[�Y���P�A�V�[�Y���Q�A�_��
	bool AddUser(INFO_USER sStudentInfo);
	bool DeleteUser(int nIndex);
	bool SetUserInfo(int nIndex, INFO_USER sStudentInfo);
	void SetPointClear(int nIndex);
public:
	CUser(void);
	~CUser(void);
};
