using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarSelectionItem : MonoBehaviour
{
    public event EventHandler<CarEventArgs> onSelect;

    [Header("UI")]
    public Text titleText;
    public Text subtitleText;
    public Image carImage;
    public Image grayscaleImage;
    public Button selectButton;
    public CanvasGroup cg;

    public Car car;

    private Color originalFontColor;

    /// <summary>
    /// Update item from Car
    /// </summary>
    public void Initialize(Car car)
    {
        this.car = car;
        this.gameObject.name += "_" + car.title;
        titleText.text = car.title;
        subtitleText.text = car.subtitle;
        string fullImagePath = GetImagePath(car.imageName);
        IOHelper.GetSprite(Main.instance, fullImagePath, (sprite) =>
        {
            carImage.sprite = sprite;
            grayscaleImage.sprite = sprite;
        });

        selectButton.onClick.AddListener(() =>
        {
            if (onSelect != null)
                onSelect(this, new CarEventArgs(car));
        });

        originalFontColor = titleText.color;
    }

    public void SetDummy()
    {
        car = new Car("dummy.model", "dummy", "sample.jpg", "");
        cg.alpha = 0;
    }

    private void FixedUpdate()
    {
        // Get relative screen position (0-1)
        Vector2 screenPosRelative = this.transform.position / new Vector2(Screen.width, Screen.height);
#if UNITY_EDITOR
        screenPosRelative = this.transform.position / IOHelper.GetMainGameViewSize();
#endif
        // Get relative distance from center
        float distance = HelperFunctions.Retarget(screenPosRelative.y, 0f, 1f, -1f, 1f);
        distance = Mathf.Abs(distance);
        // Narrow wideness
        distance *= 2f;

        grayscaleImage.color = new Color(1f, 1f, 1f, distance);
        titleText.color = Color.Lerp(originalFontColor, Color.gray, distance);
        subtitleText.color = Color.Lerp(originalFontColor, Color.gray, distance);
    }

    /// <summary>
    /// Get full image path under {Application}\\StreamingAssets\\Images\\{model}.jpg
    /// </summary>
    public string GetImagePath(string imageName)
    {
        return string.Format("{0}\\Images\\{1}", Application.streamingAssetsPath, imageName);
    }

    public class CarEventArgs : EventArgs
    {
        public Car car;
        public CarEventArgs(Car car)
        {
            this.car = car;
        }
    }

}
