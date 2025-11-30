using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scraft;
using UnityEngine;
using UnityEngine.UI;

public class SDMain : MonoBehaviour
{
    public List<SDDataCell> cells;
    public SDColSelector dtypeSelector;
    public SDColSelector utypeSelector;
    public SDColSelector sortSelector;
    public TMPro.TMP_Text pageText;
    public Button nextPageButton;
    public Button prevPageButton;
    public string sort = "all";
    public string btype = "new";
    public string utype = "all";
    public int page = 1;
    public int totalPage = 1;
    public bool loading = false;
    async void Start()
    {
        pageText.SetText(page.ToString());
        foreach (var cell in cells)
        {
            cell.SetSDData(null);
        }
        dtypeSelector.onCellChanged.AddListener(OnSDTypeChanged);
        utypeSelector.onCellChanged.AddListener(OnSDUTypeChanged);
        sortSelector.onCellChanged.AddListener(OnSDSortedChangedAsync);
        nextPageButton.onClick.AddListener(OnNextPageButtonClickAsync);
        prevPageButton.onClick.AddListener(OnPrevPageButtonClickAsync);
        await GetShipList();
    }

    public async void OnNextPageButtonClickAsync()
    {
        page++;
        if (page > totalPage)
        {
            page = totalPage;
            IToast.instance.show("已到最后一页;You have reached the last page", 100);
            return;
        }
        pageText.SetText(page.ToString());
        await GetShipList();
    }

    public async void OnPrevPageButtonClickAsync()
    {
        page--;
        if (page < 1)
        {
            page = 1;
        }            
        pageText.SetText(page.ToString());
        await GetShipList();
    }

    public async void OnSDTypeChanged(string value)
    {
        btype = value;
        await GetShipList();
    }

    public async void OnSDSortedChangedAsync(string value)
    {
        sort = value;
        await GetShipList();
    }

    public async void OnSDUTypeChanged(string value)
    {
        utype = value;
        await GetShipList();
    }

    public async Task GetShipList()
    {
        if (LoginHandle.userData == null || LoginHandle.userData.token1 == null)
        {
            AlertBox.instance.Show("请先登录\nPlease log in first");
            return;
        }
        if (loading)
            return;
        loading = true;
        IToast.instance.show("Loading...");
        var request = new HttpRequest(NetworkFactory.SCRAFT_HOST + "/ship/list");
        request.addFormData("token1", LoginHandle.userData.token1);
        request.addFormData("app_device", GameSetting.appDevice);
        request.addFormData("app_channel", GameSetting.appChannel);
        request.addFormData("app_version", Application.version);
        request.addFormData("sort", sort);
        request.addFormData("btype", btype);
        request.addFormData("utype", utype);
        request.addFormData("p", page.ToString());
        request.addFormData("pm", "5");
        HttpResponse response = await NetworkFactory.getHttpNet().PostAsync(request);
        IToast.instance.hide();
        loading = false;
        if (response.code == 200)
        {
            var jsonResponse = new HttpJsonResponse<List<SDData>>(response);
            if (jsonResponse.code > 0)
            {
                totalPage = jsonResponse.code;
                for (int i = 0; i < cells.Count; i++)
                {
                    var cell = cells[i];
                    if (i < jsonResponse.data.Count)
                    {
                        cell.SetSDData(jsonResponse.data[i]);
                    }
                    else
                    {
                        cell.SetSDData(null);
                    }
                }
            }
        }
        else
        {
            AlertBox.instance.Show(response.body);
        }
    }
}
