using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using CardData;

public class CardSheetReader : MonoBehaviour
{
    public string sheetID = "1VvFDN7ruAOAvdo753SgBvfWGQsEZlw2H6VN_t1PWtgs";
    public string[] sheetNames = {"���[�_�[","�q�[���[","�����X�^�[","�A�C�e��","�}�W�b�N","Modifier"};
    public bool IsLoading { get; private set; }
    public string[][] Datas { get; private set; }
    public CardDataManager cdm = new CardDataManager();
    public UnityEvent callBack= new UnityEvent();
    IEnumerator GetFromWeb() {
        IsLoading = true;
        var tqx = "tqx=out:csv";
        var url = "https://docs.google.com/spreadsheets/d/" + sheetID + "/gviz/tq?" + tqx + "&sheet=" + sheetNames[0];
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        IsLoading = false;
        var protocol_error = request.result == UnityWebRequest.Result.ProtocolError ? true : false;
        var connection_error = request.result == UnityWebRequest.Result.ConnectionError ? true : false;
        if (protocol_error || connection_error) Debug.LogError(request.error);
        else Datas = ConvertCSVtoJaggedArray(request.downloadHandler.text);
        cdm.LoadLeader(Datas);

        url = "https://docs.google.com/spreadsheets/d/" + sheetID + "/gviz/tq?" + tqx + "&sheet=" + sheetNames[1];
        request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        IsLoading = false;
        protocol_error = request.result == UnityWebRequest.Result.ProtocolError ? true : false;
        connection_error = request.result == UnityWebRequest.Result.ConnectionError ? true : false;
        if (protocol_error || connection_error) Debug.LogError(request.error);
        else Datas = ConvertCSVtoJaggedArray(request.downloadHandler.text);
        cdm.LoadHeroCard(Datas);

        url = "https://docs.google.com/spreadsheets/d/" + sheetID + "/gviz/tq?" + tqx + "&sheet=" + sheetNames[2];
        request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        IsLoading = false;
        protocol_error = request.result == UnityWebRequest.Result.ProtocolError ? true : false;
        connection_error = request.result == UnityWebRequest.Result.ConnectionError ? true : false;
        if (protocol_error || connection_error) Debug.LogError(request.error);
        else Datas = ConvertCSVtoJaggedArray(request.downloadHandler.text);
        cdm.LoadMonsterCard(Datas);

        url = "https://docs.google.com/spreadsheets/d/" + sheetID + "/gviz/tq?" + tqx + "&sheet=" + sheetNames[3];
        request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        IsLoading = false;
        protocol_error = request.result == UnityWebRequest.Result.ProtocolError ? true : false;
        connection_error = request.result == UnityWebRequest.Result.ConnectionError ? true : false;
        if (protocol_error || connection_error) Debug.LogError(request.error);
        else Datas = ConvertCSVtoJaggedArray(request.downloadHandler.text);
        cdm.LoadItemCard(Datas);

        url = "https://docs.google.com/spreadsheets/d/" + sheetID + "/gviz/tq?" + tqx + "&sheet=" + sheetNames[5];
        request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        IsLoading = false;
        protocol_error = request.result == UnityWebRequest.Result.ProtocolError ? true : false;
        connection_error = request.result == UnityWebRequest.Result.ConnectionError ? true : false;
        if (protocol_error || connection_error) Debug.LogError(request.error);
        else Datas = ConvertCSVtoJaggedArray(request.downloadHandler.text);
        cdm.LoadModifierCard(Datas);

        url = "https://docs.google.com/spreadsheets/d/" + sheetID + "/gviz/tq?" + tqx + "&sheet=" + sheetNames[4];
        request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        IsLoading = false;
        protocol_error = request.result == UnityWebRequest.Result.ProtocolError ? true : false;
        connection_error = request.result == UnityWebRequest.Result.ConnectionError ? true : false;
        if (protocol_error || connection_error) Debug.LogError(request.error);
        else Datas = ConvertCSVtoJaggedArray(request.downloadHandler.text);
        cdm.LoadMagicCard(Datas);


        cdm.LoadChallengeCard();

        callBack.Invoke();
    }
    public void Load() {
        StartCoroutine(GetFromWeb());
    }

    static string[][] ConvertCSVtoJaggedArray(string t) {
        var reader = new StringReader(t);
        reader.ReadLine();  //ヘッダ読み飛ばし
        var rows = new List<string[]>();
        while (reader.Peek() >= 0) {
            var line = reader.ReadLine();        //一行ずつ読込
            var elements = line.Split(',');    //行のセルは,で区切られる
            for (var i = 0; i < elements.Length; i++) {
                elements[i] = elements[i].TrimStart('"').TrimEnd('"');
            }
            rows.Add(elements);
        }
        return rows.ToArray();
    }
}
