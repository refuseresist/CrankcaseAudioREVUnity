using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CarSelectionManager : MonoBehaviour
{
    public event EventHandler<CarSelectionItem.CarEventArgs> onSelectItem;

    [Header("UI")]
    public Button backButton;
    public GameObject carSelectionItemPrefab;
    public GameObject carSelectionParent;
    public ScrollRect scrollRect;
    public string configFilename = "config.csv";

    private List<Car> cars = new List<Car>();
    private List<CarSelectionItem> carItems = new List<CarSelectionItem>();
    
    public void Initialize()
    {
        backButton.onClick.AddListener(BackButton_OnClick);
        string configFile = string.Format("{0}\\{1}", Application.streamingAssetsPath, configFilename);
        cars = IOHelper.ReadConfigFile(configFile);
        CreateCarList();
        ScrollTo(1);
        this.gameObject.SetActive(false);
    }

    public void ScrollTo(int index)
    {
        float segment = 1f / (scrollRect.content.transform.childCount - 4);
        scrollRect.verticalScrollbar.value = 1 - segment * index;
    }

    public void Toggle(bool show)
    {
        this.gameObject.SetActive(show);
    }

    private void CreateCarList()
    {
        ClearParentContainer();
        carItems.Clear();

        for (int i = 0; i < cars.Count; i++)
        {
            // Create GameObject listing
            CarSelectionItem item = GameObject.Instantiate(carSelectionItemPrefab).GetComponent<CarSelectionItem>();

            // Update details
            item.Initialize(cars[i]);
            item.onSelect += Item_onSelect;
            item.transform.SetParent(carSelectionParent.transform);

            carItems.Add(item);
        }

        // Create GameObject listing
        CarSelectionItem item1 = GameObject.Instantiate(carSelectionItemPrefab).GetComponent<CarSelectionItem>();
        item1.SetDummy();
        item1.transform.SetParent(carSelectionParent.transform);
        item1.transform.SetAsFirstSibling();

        // Create GameObject listing
        CarSelectionItem item2 = GameObject.Instantiate(carSelectionItemPrefab).GetComponent<CarSelectionItem>();
        item2.SetDummy();
        item2.transform.SetParent(carSelectionParent.transform);
        item2.transform.SetAsLastSibling();
    }

    private void Item_onSelect(object sender, CarSelectionItem.CarEventArgs e)
    {
        if(onSelectItem != null)
        {
            onSelectItem(sender, e);
        }
    }

    private void BackButton_OnClick()
    {
        Toggle(false);
    }

    /// <summary>
    /// Delete all children in parent container
    /// </summary>
    private void ClearParentContainer()
    {
        // Find all children
        List<Transform> children = new List<Transform>();
        foreach (Transform child in carSelectionParent.transform)
        {
            children.Add(child);
        }

        // Delete all children
        for (int i = 0; i < children.Count; i++)
        {
            Destroy(children[i].gameObject);
        }
    }
}
