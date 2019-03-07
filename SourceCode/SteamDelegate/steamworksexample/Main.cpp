//====== Copyright � 1996-2008, Valve Corporation, All rights reserved. =======
//
// Purpose: Main file for the SteamworksExample app
//
//=============================================================================

#include "stdafx.h"
#include "steam/steam_api.h"
#ifdef WIN32
#include <direct.h>
#else
#define MAX_PATH PATH_MAX
#include <unistd.h>
#define _getcwd getcwd
#endif

//-----------------------------------------------------------------------------
// Purpose: Main entry point for the program -- win32
//-----------------------------------------------------------------------------
#ifdef WIN32

#include <iostream>
#include <Windows.h>
#include <string>
#include <codecvt>
#include <sstream>
#pragma comment(lib, "urlmon.lib")

char* U2G(const char* utf8)
{
	int len = MultiByteToWideChar(CP_UTF8, 0, utf8, -1, NULL, 0);
	wchar_t* wstr = new wchar_t[len+1];
	memset(wstr, 0, len+1);
	MultiByteToWideChar(CP_UTF8, 0, utf8, -1, wstr, len);
	len = WideCharToMultiByte(CP_ACP, 0, wstr, -1, NULL, 0, NULL, NULL);
	char* str = new char[len+1];
	memset(str, 0, len+1);
	WideCharToMultiByte(CP_ACP, 0, wstr, -1, str, len, NULL, NULL);
	if(wstr) delete[] wstr;
	return str;
}

void Replace(std::string &str, const std::string &o, const std::string &n)
{
	std::size_t pos = str.find(o);
	while(pos != std::string::npos)
	{
		str.replace(pos, o.length(), n);
		pos = str.find(o, pos + n.length());
	}
}

std::ostream& SerializeByU2G(std::ostream &ss, const char* utf8)
{
	char* cs = U2G(utf8);
	//std::string str(cs);
	//Replace(str, "\\", "\\\\\\\\");
	//Replace(str, "\r", "\\\\r");
	//Replace(str, "\n", "\\\\n");
	//Replace(str, "\"", "\\\\\\\"");
	//ss << str;
	ss << cs;
	delete[] cs;
	return ss;
}

void ConvertJson(std::string &str)
{
	Replace(str, "\\", "\\\\");
	Replace(str, "\r", "\\r");
	Replace(str, "\n", "\\n");
	Replace(str, "\"", "\\\"");
}

std::string GetConvertJson(const char* cs)
{
	std::string ret(cs);
	ConvertJson(ret);
	return ret;
}

#include <iomanip>
#include <fstream>
void WriteAllText(const char* file, const char* text)
{
	std::ofstream of(file, std::ios::out);
	of << text;
	of.flush();
	of.close();
}

const char* path = "cache/";
void SerializeUGCDetail(std::ostream& ss, SteamUGCDetails_t &detail, char* url)
{
	std::string id = std::to_string(detail.m_nPublishedFileId);
	std::ofstream des(path + id + ".des", std::ios::out);
	std::ofstream json(path + id + ".json", std::ios::out);
	json << "{\"FileSize\":" << detail.m_nFileSize;
	json << ",\"PublishedId\":" << detail.m_nPublishedFileId;
	if(url != NULL)
		json << ",\"ImageURL\":\"" << url << "\"";
	json << ",\"Title\":\"" << GetConvertJson(detail.m_rgchTitle) << "\"";
	json << ",\"URL\":\"" << GetConvertJson(detail.m_rgchURL) << "\"";
	json << ",\"OwnerId\":\"" << detail.m_ulSteamIDOwner << "\"";
	json << ",\"Score\":\"" << detail.m_flScore << "\"";
	json << ",\"Tags\":[\"" << detail.m_rgchTags << "\"]}"; //need be separated
	des << detail.m_rgchDescription;
	json.flush();
	json.close();
	des.flush();
	des.close();
	ss << detail.m_nPublishedFileId << std::endl;
	return;
	ss << "{\\\"FileSize\\\":" << detail.m_nFileSize;
	ss << ",\\\"PublishedId\\\":" << detail.m_nPublishedFileId;
	if(url != NULL)
		ss << ",\\\"ImageURL\\\":\\\"" << url << "\\\"";
	ss << ",\\\"Title\\\":\\\"";
	SerializeByU2G(ss, detail.m_rgchTitle) << "\\\"";
	ss << ",\\\"Description\\\":\\\"";
	SerializeByU2G(ss, detail.m_rgchDescription) << "\\\"";
	ss << ",\\\"Tags\\\":[\\\"";
	SerializeByU2G(ss, detail.m_rgchTags) << "\\\"]}"; //need be separated
}

void Fault()
{
	std::cout << "{\"Result\":false}" << std::endl;
}

void Success()
{
	std::cout << "{\"Result\":true}" << std::endl;
}

PublishedFileId_t Convert2Id(char* str)
{
    PublishedFileId_t result;
	std::stringstream ss;
    ss << str;
    ss >> result;
    return result;
}

bool Flow(int argc, char* argv[])
{
	if(!SteamAPI_Init())
	{
		std::cout << "1" << std::endl;
		return false;
	}
	else
	{
		std::cout << "1" << std::endl;
	}
	if(SteamUtils()->GetAppID() != 550)
		return false;
	uint32 num = argc;
	//num = SteamUGC()->GetNumSubscribedItems();
	if(num > 0)
	{
		PublishedFileId_t* pvecPublishedFileID = new PublishedFileId_t[num];
		//num = SteamUGC()->GetSubscribedItems(pvecPublishedFileID, num);
		for(int i = 0; i < argc; i++)
			pvecPublishedFileID[i] = Convert2Id(argv[i]);
		UGCQueryHandle_t ugcQueryHandle = SteamUGC()->CreateQueryUGCDetailsRequest(pvecPublishedFileID, num);
		delete[] pvecPublishedFileID;
		SteamAPICall_t call = SteamUGC()->SendQueryUGCRequest(ugcQueryHandle);
		bool failed = true;
		while(true)
		{
			bool complete = SteamUtils()->IsAPICallCompleted( call, &failed );
			if(complete)
			{
				if(failed)
					return false;
				SteamUGCQueryCompleted_t ugcqc;
				SteamUtils()->GetAPICallResult( call, &ugcqc, sizeof(ugcqc), ugcqc.k_iCallback, &failed );
				if(ugcqc.m_eResult != k_EResultOK || failed)
					return false;
				Success();
				if(ugcqc.m_unNumResultsReturned == 0)
					return true;
				//std::cout << "{\"Result\":true,\"Mods\":[";
				char* pchURL = new char[256];
				bool first = true;
				for(uint32 i = 0; i < ugcqc.m_unNumResultsReturned; i++)
				{
					SteamUGCDetails_t detail;
					if(SteamUGC()->GetQueryUGCResult(ugcqc.m_handle, i, &detail))
					{
						if(detail.m_nFileSize > 0)
						{
							if(first)
								first = false;
							//else
							//	std::cout << ',';
							bool haveURL = SteamUGC()->GetQueryUGCPreviewURL(ugcqc.m_handle, i, pchURL, 256);
							//std::cout << '"';
							SerializeUGCDetail(std::cout, detail, haveURL ? pchURL : NULL);
							//std::cout << '"';
						}
					}
				}
				//std::cout << "]}" << std::endl;
				delete[] pchURL;
				break;
			}
		}
	}
	else
		Success();
	return true;
}

int main(int argc, char* argv[])
{
	if(!Flow(argc - 1, argv + 1))
		Fault();
	else 
		std::cout << 0 << std::endl;
	return 0;
}
#endif

