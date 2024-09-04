using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSocketUI : MonoBehaviour
{
    public int weaponIndex = 0;
    private Weapon weapon;

    [SerializeField] private TextMeshProUGUI ammoTxt, prepareAmmoTxt;
    [SerializeField] private RectTransform reloadMask;
    [SerializeField] private CanvasGroup textCanvas;

    [Serializable]
    public class SwitchObject
    {
        public Image img;
        public float minAlpha = 0.2f, maxAlpha = 1f;
    }
    [SerializeField] private List<SwitchObject> needSwitchWhenSelected;
    [SerializeField] private float minScale = 0.7f, maxScale = 1f;
    private Vector2 initSizeDelta;


    //Debug
    [SerializeField] private Text stateText;

    private void Awake()
    {
        initSizeDelta = GetComponent<RectTransform>().sizeDelta;
    }

    // Start is called before the first frame update
    void Start()
    {
        weapon = PlayerController.Ins.weapons[weaponIndex];


        weapon.onAmmoChanged += (curAmmo, curPrepareAmmo) =>
        {
            ammoTxt.text = curAmmo.ToString();
            prepareAmmoTxt.text = curPrepareAmmo.ToString();
        };
        ammoTxt.text = weapon.GetCurAmmo().ToString();
        prepareAmmoTxt.text = weapon.GetCurPrepareAmmo().ToString();


        weapon.onReloading += (remain) =>
        {
            Vector3 scale = reloadMask.localScale;
            scale.x = remain / weapon.GetReloadTime();
            reloadMask.localScale = scale;
        };


        weapon.onSelectChanged += SetActive;
        SetActive(weapon.selected);
    }

    private void SetActive(bool selected)
    {
        //ÑÕÉ«
        foreach (var obj in needSwitchWhenSelected)
        {
            Color c = obj.img.color;
            c.a = selected ? obj.maxAlpha : obj.minAlpha;
            obj.img.color = c;
        }
        textCanvas.alpha = selected ? 1 : 0.2f;

        //´óÐ¡
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = selected ? initSizeDelta * maxScale : initSizeDelta * minScale;

        textCanvas.GetComponent<RectTransform>().localScale = selected ? Vector3.one : Vector3.one / 2;
    }

    private void Update()
    {
        stateText.text = weapon.GetCurState().ToString();
    }
}
