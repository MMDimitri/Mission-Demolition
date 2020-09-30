using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    static private Slingshot S;


    public Transform leftAnchor;
    public Transform rightAnchor;

    public LineRenderer[] rubberBand;

   
    [Header("Set in Inspector")]
    public GameObject prefabProjectile;
    public float velocityMult = 8f;

    [Header("Set Dynamically")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    private Rigidbody projectileRigidbody;


    static public Vector3 LAUNCH_POS
    {
        get
        {
            if (S == null) return Vector3.zero;
            return S.launchPos;
        }
    }

    void Awake()
    {
        S = this;
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }

    //void Start()
    //{
    //    rubberBand[0].SetPosition(0, leftAnchor.position);
    //    rubberBand[1].SetPosition(0, rightAnchor.position);
    //}

    void OnMouseEnter()
    {
        //print("Slingshot:OnMouseEnter()");
        launchPoint.SetActive(true);
    }

    void OnMouseExit()
    {
        //print("Slingshot:OnMouseExit()");
        launchPoint.SetActive(false);
    }

    void OnMouseDown()
    {
        aimingMode = true;
        projectile = Instantiate(prefabProjectile) as GameObject;
        projectile.transform.position = launchPos;

        projectileRigidbody = projectile.GetComponent<Rigidbody>();
        projectileRigidbody.isKinematic = true;
        rubberBand[0].SetPosition(0, leftAnchor.position);
        rubberBand[1].SetPosition(0, rightAnchor.position);
    }

    void Update()
    {
        if (!aimingMode) return;

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);


        Vector3 mouseDelta = mousePos3D - launchPos;

        float maxMagnitude = this.GetComponent<SphereCollider>().radius;

        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        for (int i = 0; i < rubberBand.Length; i++)
        {
            rubberBand[i].SetPosition(1, projPos);
        }

        if ( Input.GetMouseButtonUp(0) )
        {
            aimingMode = false;
            projectileRigidbody.isKinematic = false;
            projectileRigidbody.velocity = -mouseDelta * velocityMult;
            FollowCam.POI = projectile;
            MissionDemolition.ShotFired();
            ProjectileLine.S.poi = projectile;
            projectile = null;
            GetComponent<AudioSource>().Play();
            Destroy(rubberBand[0]);
            Destroy(rubberBand[1]);
        }
    }
}
