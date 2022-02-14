using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunUIManager : MonoBehaviour
{
    public PlayerShooter ps;
    //public Sprite bulletSprite;
    public int maxBullets = 100;
    public RectTransform bulletBar;
    public GridLayoutGroup bulletGrid;
    public TextMeshProUGUI gunName;
    public TextMeshProUGUI totalBullets;
    public TextMeshProUGUI leftBullets;
    public List<GameObject> bulletPieces;
    public GameObject bulletPiecePrefab;

    // Start is called before the first frame update
    public void StartGuns()
    {
        bulletPieces = new List<GameObject>();
        for (int i = 0; i < maxBullets; i++)
        {
            GameObject go = Instantiate(bulletPiecePrefab, bulletBar.transform);
            go.SetActive(false);
            bulletPieces.Add(go);
        }
        UpdateGunUI();
    }

    public void UpdateGunUI()
    {
        gunName.text = ps.gunName;
        leftBullets.text = ps.ammoLeft.ToString();
        if (ps.ammoTotal == -1)
            totalBullets.text = "\u221E";
        else
            totalBullets.text = ps.ammoTotal.ToString();
        int lb = ps.ammoLeft;
        int mb = ps.ammoSize;
        float w = bulletBar.sizeDelta.x;
        float h = bulletBar.sizeDelta.y;
        float bw = w / mb;
        bulletGrid.cellSize = new Vector2(bw, h);
        // Debug.Log(bulletPieces.Count + " " + maxBullets);
        for (int i = 0; i < maxBullets; i++)
        {
            if (i < lb)
            {
                bulletPieces[i].SetActive(true);
            }
            else
            {
                bulletPieces[i].SetActive(false);
            }
        }
    }
}
