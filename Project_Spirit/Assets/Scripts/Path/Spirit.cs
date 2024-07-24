 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : MonoBehaviour
{
    public int SpiritID;
    public int SpiritJob;
    public int SpiritElement;
    public string type;
    public float SDefaultLife;
    public float HP;
    public float SpiritSpeed;
    public float Work_Efficienty;
    string SpiritName;

    GameObject CradleManager;
    private void Start()
    {
        SDefaultLife = 100f;
        HP = SDefaultLife;
        CradleManager = GameObject.Find("CradleManager");
        check();
    }

    public void TakeBuildingExpense()
    {

    }

    // ���� �Ʒü�, ������ ���� ����Ǵ� ȿ��
    public void TakeDamageOfBuilding()
    {
        SDefaultLife *= (1 - 0.2f);
        HP *= (1 - 0.2f);
    }
    public void TakeDamageOfResourceBuilding()
    {        
        SDefaultLife *= SpiritManager.instance.resourceBuildingDamagePercent; ;
        HP *= SpiritManager.instance.resourceBuildingDamagePercent; ;
    }
    public void TakeAdvantageOfBuilding()
    {
        SDefaultLife *= 4;
        HP *= 4;
    }
    public int GetSpiritID()
    {
        return SpiritID; 
    }
    public void SetSpiritID(int _SpiritID)
    {
        Debug.Log(SpiritID);
        SpiritID = _SpiritID;
    }

    void check()
    {
        switch(SpiritElement)
        {
            case 1:
                type = "Fire";
                break;
            case 2:
                type = "Water";
                break;
            case 3:
                type = "Ground";
                break;
            case 4:
                type = "Air";
                break;
        }
    }
    public void DevoteToCradle()
    {
        CradleManager.GetComponent<CradleManager>().AddElement(type, (int)HP);
        Destroy(this.gameObject);
    }
    #region �����浹 ����
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Spirit")
        {
            int element = collision.gameObject.GetComponent<Spirit>().SpiritElement;
            if(element != SpiritElement ) 
            {
                // ��� ������ ��Ȳ.
                if (SpiritID == 4)
                {
                    Destroy(collision.gameObject);
                }
                // ��� ������ �ƴ� ��Ȳ.
                else
                {
                    if (collision.gameObject.GetComponent<Spirit>().HP >= HP)
                    {
                        collision.gameObject.GetComponent<Spirit>().HP -= HP;
                        if (collision.gameObject.GetComponent<Spirit>().HP <= 0)
                        {
                            Destroy(gameObject);
                        }
                        
                    }
                    else
                    {
                        HP -= collision.gameObject.GetComponent<Spirit>().HP;
                        Destroy(collision.gameObject);
                    }
                }
            }
            
        }
    }
    #endregion
}
