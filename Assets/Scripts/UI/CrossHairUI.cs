using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CrossHairUI : MonoBehaviour
{
    //×ÔÉí
    private RectTransform rect, parent;

    [SerializeField] private GameObject aimHair, reloadHair;
    [SerializeField] private TextMeshPro reloadTxt;

    //ÎäÆ÷
    public int weaponIndex = 0;
    private Weapon weapon;
    private Transform fireSocket;

    public float lerpMultipler = 0.1f;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        parent = rect.parent.GetComponent<RectTransform>();

        weapon = PlayerController.Ins.weapons[weaponIndex];
        fireSocket = weapon.fireSocket;
        weapon.onReloadStart += OnReloadStart;
        weapon.onReloading += OnReloading;
        weapon.onReloadEnd += OnReloadEnd;

        weapon.onSelectChanged += (selected) =>
        {
            gameObject.SetActive(selected);
        };

        gameObject.SetActive(weapon.selected);
    }

    void Update()
    {
        if (Physics.SphereCast(fireSocket.position, .5f, fireSocket.forward, out RaycastHit hitInfo, 1000))
        {
            if(RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, RectTransformUtility.WorldToScreenPoint(Camera.main, hitInfo.point), Camera.main, out Vector2 localPoint))
            {
                rect.localPosition = Vector2.Lerp(rect.localPosition, localPoint, lerpMultipler);
            }
        }
        else
        {
            Vector3 point = fireSocket.position + fireSocket.forward * 3000;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, RectTransformUtility.WorldToScreenPoint(Camera.main, point), Camera.main, out Vector2 localPoint))
            {
                rect.localPosition = Vector2.Lerp(rect.localPosition, localPoint, lerpMultipler);
            }
        }
    }

    private void OnReloadStart()
    {
        aimHair.SetActive(false);
        reloadHair.SetActive(true);
    }

    private void OnReloading(float remainTime)
    {
        reloadTxt.SetText(remainTime.ToString("0.00"));
    }

    private void OnReloadEnd()
    {
        aimHair.SetActive(true);
        reloadHair.SetActive(false);
    }
}
