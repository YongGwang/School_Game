#include "User.h"


CUser::CUser(void)
{
}


CUser::~CUser(void)
{
}

//IDだけ渡すとファイルローディング
bool CUser::Load()
{
	FILE* folderdirection;
	INFO_USER sStudentInfo;

	//ファイルの名前とどう使えるか指定
	folderdirection = fopen("user.dat", "rb");
	if (folderdirection != NULL)
	{
		mUserList.clear();
		while (!feof(folderdirection))
		{
			fread(&sStudentInfo, sizeof(INFO_USER), 1, folderdirection);
			mUserList.push_back(sStudentInfo);
		}
		fclose(folderdirection);
	}
	else {
		return false;
	}
	return true;
}

bool CUser::Save()
{
	FILE* folderdirection;
	INFO_USER sStudentInfo;
	folderdirection = fopen("user.dat", "wb");

	list<INFO_USER>::iterator pos;
	for (pos = mUserList.begin(); pos != mUserList.end(); pos++)
	{
		sStudentInfo = *pos;
		fwrite(&sStudentInfo, sizeof(INFO_USER), 1, folderdirection);
	}

	fclose(folderdirection);
	return true;
}

//IDで検索
int CUser::Find(char* pNumber, INFO_USER* pStudentInfo)
{
	int nIndex;
	INFO_USER sStudentInfo;
	list<INFO_USER>::iterator pos;
	for (pos = mUserList.begin(), nIndex = 0; pos != mUserList.end(); pos++, nIndex++)
	{
		if (strcmp(pNumber, pos->sNum) == 0)
		{
			sStudentInfo = *pos;
			memcpy(pStudentInfo, &sStudentInfo, sizeof(INFO_USER));
			return nIndex;
		}
	}

	return -1;
}

//検索
int CUser::Find(char* pNumber)
{
	int nIndex;
	INFO_USER sUserInfo;
	list<INFO_USER>::iterator pos;
	for (pos = mUserList.begin(), nIndex = 0; pos != mUserList.end(); pos++, nIndex++)
	{
		if (strcmp(pNumber, pos->sNum) == 0)
		{
			return nIndex;
		}
	}

	return -1;
}

// シーズン１、シーズン２、点数
bool CUser::SetScore(int nIndex, int First, int Late, int nScore)
{
	list<INFO_USER>::iterator pos = mUserList.begin();

	for (int i = 0; i < nIndex; i++)
		pos++;
	pos->nPoints[First][Late] = nScore;
	return true;
}

// 追加
bool CUser::AddUser(INFO_USER sStudentInfo)
{
	FILE* folderdirection;
	mUserList.push_back(sStudentInfo);

	folderdirection = fopen("user.dat", "ab");
	fwrite(&sStudentInfo, sizeof(INFO_USER), 1, folderdirection);
	fclose(folderdirection);
	return true;
}

//削除
bool CUser::DeleteUser(int nIndex)
{
	FILE* folderdirection;
	INFO_USER sUserInfo;
	list<INFO_USER>::iterator pos = mUserList.begin();
	for (int i = 0; i < nIndex; i++)
		pos++;
	mUserList.erase(pos);
	Save();
	return true;
}

//修正
bool CUser::SetUserInfo(int nIndex, INFO_USER sStudentInfo)
{
	list<INFO_USER>::iterator pos = mUserList.begin();
	for (int i = 0; i < nIndex; i++)
		pos++;
	*pos = sStudentInfo;
	Save();
	return true;
}

//未実装：ランクシステム
void CUser::SetPointClear(int nIndex)
{
	list<INFO_USER>::iterator pos = mUserList.begin();
	for (int i = 0; i < nIndex; i++)
		pos++;

	memset(pos->nPoints, -1, sizeof(pos->nPoints));
}