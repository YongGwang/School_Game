
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

	//�S�̃f�[�^������������Ƃ���
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
	//��������ϊ�
	printf("������������������������������������������������������������������������������������������������������������������������\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�������悤�Ƃ��Ă���f�[�^�x�[�X�̐�����͂��Ă��������@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@��������f�[�^�x�[�X�̐��F�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("������������������������������������������������������������������������������������������������������������������������\n");

	gotoxy(45, 5);
	scanf("%d", &g_nDataTotal);
	rewind(stdin);
	//memory��O������300�܂�
	ExceptHandleMemNum(&g_nDataTotal);
	g_pDatabase = (ACOUNT*)malloc(sizeof(ACOUNT)*g_nDataTotal);

	system("cls");
	printf("������������������������������������������������������������������������������������������������������������������������\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�������悤�Ƃ��Ă���ID������͂��Ă��������@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("������������������������������������������������������������������������������������������������������������������������\n");

	//�f�[�^�x�[�X�̐��ɓ����郁�����[����
	for (i = 0; i < g_nDataTotal; ++i)
	{
		gotoxy(4, 5);
		printf("�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@"); //�ꌅ������
		gotoxy(6, 5);
		printf("�f�[�^�x�[�X�ԍ��@%d�@�@�A�J�E���g�̐��F ", i + 1);
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

	printf("������������������������������������������������������������������������������������������������������������������������\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@���@�A�J�E���g�Ǘ������v���O�����@���@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�O�D�������@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�P�D���́@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�Q�D�C���@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�R�D�����@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�S�D�o�́@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�T�D�I���@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@���j���[��I�����Ă��������@�i�@�@�@�@�@�@�j�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("������������������������������������������������������������������������������������������������������������������������\n");

	gotoxy(46, 16);
	scanf("%d", &nKey);
	return nKey;
}

void AcountInfoDisplay(int nIndex)
{
	char* strTitle[4] = { "���@�f�[�^���́@��", "���@�f�[�^�C���@��", "���@�f�[�^�����@��" };
	system("cls");
	printf("������������������������������������������������������������������������������������������������������������������������\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�f�[�^�x�[�X�F�@�@�@�@�@�@�@�@�A�J�E���g�ԍ��F�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�E�Ɣԍ��@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���O�DAssassin�P�DKnight�Q�DDoctor�R�DNinja�S�DMonstertamer ��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@���O�F�@�@�@�@�@�@�@�@�@�E�ƁF�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@���\�͒l�@�@�\�͒l��3000�܂œ��͂ł��܂��@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�X�g�����O�X�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�A�W���e�B�[�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�m�b�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�^�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("������������������������������������������������������������������������������������������������������������������������\n");
	gotoxy(20, 1);
	printf("%s", strTitle[nIndex]);
}

void Input()
{
	char chYESorNO;
	ACOUNT sAcount;
	int nDatabase, nNum;
	int jobTemp;

	//�Ԉ���ē��͂��ꂽ�Ƃ����j���[�ɖ߂�
	while (1)
	{
		system("cls");
		AcountInfoDisplay(0);

		//�f�[�^�x�[�X�ԍ���1�Ԃ���
		gotoxy(20, 4);
		scanf("%d", &nDatabase);
		//�������[���������f�[�^�x�[�X�ȊO�̔ԍ��A���������͂�����
		rewind(stdin);
		if (nDatabase > g_nDataTotal || 0 > nDatabase)
			return;

		gotoxy(3, 3);
		printf("��%d�@�f�[�^�x�[�X�̑����͉\�ȃA�J�E���g��%d�ł��B", nDatabase, g_pDatabase[nDatabase - 1].nAcountCount); //�ǉ�
		gotoxy(52, 4);
		scanf("%d", &nNum);
		rewind(stdin);
		if (nNum > g_pDatabase[nDatabase -1].nAcountCount || 0 > nNum)
			return;

		gotoxy(13, 9);
		scanf("%s", sAcount.strName);
		rewind(stdin);

		//�O�`4�܂�
		//Assassin,Knight,Doctor,Ninja,Monstertamer
		gotoxy(36, 9);
		scanf("%d", &jobTemp);
		if (jobTemp < 0 || jobTemp >4)	jobTemp = 0;
		sAcount.eJob = (JOB)jobTemp;
		rewind(stdin);

		//�\�͒l��3000�܂œ��͂ł���B
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
		printf("�����퓬�� : %d", sAcount.nCombatPower);

		gotoxy(16, 24);
		printf("�ۑ����܂����HY/N?[      ]");
		gotoxy(37, 24);
		scanf("%c", &chYESorNO);
		rewind(stdin);
		
		if (chYESorNO == 'n')
		{
			return ;
		}
		else
		{
			g_pDatabase[nDatabase - 1].pAcount[nNum - 1] = sAcount; //�C���f�b�N�X�͂O����n�߂�

			while (1)
			{
				gotoxy(16, 26);
				printf("�����܂����H�@Y/N?[      ]");
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
	printf("������������������������������������������������������������������������������������������������������������������������\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@���@�f�[�^�����@���@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�f�[�^�x�[�X:�@�@�@�@�@ �A�J�E���g�ԍ�:�@�@�@�@�@�@�@�@�@��\n");
	printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
	printf("������������������������������������������������������������������������������������������������������������������������\n");

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
		printf("�����퓬�� : %d", sAcount.nCombatPower);

		//�C������
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
		printf("�ۑ����܂����HY/N?[      ]");
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
				printf("�����܂����H�@Y/N?[      ]");
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
		printf("�����퓬�� : %d", g_pDatabase[nDatabase - 1].pAcount[nNum - 1].nCombatPower);

		while (1)
		{
			gotoxy(20, 30);
			printf("�����܂����H Y/N[�@�@�@�@�@�@]");
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
		printf("������������������������������������������������������������������������������������������������������������������������\n");
		printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
		printf("���@�@�@�@�@�@�@�@�@�@�o�́F�f�[�^�����@�@�@�@�@�@�@�@�@�@�@��\n");
		printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
		printf("���@�@�f�[�^�x�[�X�ԍ��F�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
		printf("���@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@��\n");
		printf("������������������������������������������������������������������������������������������������������������������������\n");

		gotoxy(25, 4);
		scanf("%d", &nDatabase);
		rewind(stdin);

		system("cls");
		printf("�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@���@%d �̃f�[�^�x�[�X�@��\n", nDatabase);
		printf("\n������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������\n");
		printf("�A�J�E���g�ԍ��@�@�@�@�@���O�@�@�@�@�E�Ɓ@�@�X�g�����O�X�@�@�A�W���e�B�[�@�m�b�@�@�@�^�@�@�@�����퓬��");
		printf("\n������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������\n");
		if (g_pDatabase == NULL)
		{
			printf("%d�@�̃f�[�^�x�[�X����������ĂȂ��ł��B", nDatabase);
		}
		else
		{
			for (i = 0; i < g_pDatabase[nDatabase - 1].nAcountCount; ++i)
			{
				if ((g_pDatabase + i) != NULL)
				{
					printf("�@�@%2d�@ %20s�@�@�@�@%d�@�@�@�@%4d�@�@�@�@�@�@%4d�@�@�@�@%4d�@�@�@%4d�@�@�@%6d\n", i + 1,
						g_pDatabase[nDatabase - 1].pAcount[i].strName, g_pDatabase[nDatabase - 1].pAcount[i].eJob,
						g_pDatabase[nDatabase - 1].pAcount[i].nStrenth, g_pDatabase[nDatabase - 1].pAcount[i].nAgility,
						g_pDatabase[nDatabase - 1].pAcount[i].nWisdom, g_pDatabase[nDatabase - 1].pAcount[i].nLuck,
						g_pDatabase[nDatabase - 1].pAcount[i].nCombatPower);
				}
				nCursorY++;
			}
		}
		printf("\n������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������\n");

		while (1)
		{
			printf("�@�@�@�@�@�@�@�@�@�@�@�@�@�@�����܂����H Y/N[�@�@�@�@�@�@]");
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
		case 0:	//������
			Init();
			break;
		case 1:	//����
			Input();
			break;
		case 2:	//�C��
			Modify();
			break;
		case 3:	//����
			Search();
			break;
		case 4:	//�o��
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