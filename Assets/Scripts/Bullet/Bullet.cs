using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Physical")]
    private Rigidbody rb;
    private Vector3 lastPosition;

    [Header("Attribute")]
    //�ӵ�������ϵ�������趨�Ƿ����Ѿ��˺���-1Ϊ������ϵ
    public int flag = -1;
    public float velocity = 100;
    public bool constantSpeed = false;
    public float lifeTime = 6f;
    private float lifeTimer;

    public float gravityMultiper = 1f;

    [Header("VFX")]
    public GameObject explosionPrefab;
    public GameObject trailPrefab;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
        lifeTimer = lifeTime;
    }

    public void Init(float velocity, bool constantSpeed, Vector3 inertialVelocity /*������*/, int flag = -1)
    {
        this.velocity = velocity;
        this.constantSpeed = constantSpeed;

        rb.velocity += transform.forward * velocity * Time.fixedDeltaTime / rb.mass;
        rb.AddForce(transform.forward * velocity + inertialVelocity, ForceMode.Impulse);

        this.flag = flag;
    }

    private void Update()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer < 0)
            OnCollisionEnter(null);
    }

    void FixedUpdate()
    {
        //��ֹ������壬�Լ��������ļ����һ�����߼��
        if (Physics.Raycast(lastPosition, rb.velocity.normalized, out RaycastHit hitInfo, rb.velocity.magnitude * Time.fixedDeltaTime))
        {
            transform.position = hitInfo.point;
            rb.velocity = Vector3.zero;
            return;
        }

        //�־ö���
        if (constantSpeed)
            rb.AddForce(transform.forward * velocity);

        //������ת����
        transform.forward = rb.velocity.normalized;

        //Ӧ����������
        rb.velocity += new Vector3(0, 9.8f * (1 - gravityMultiper) * Time.fixedDeltaTime, 0);

        //��¼λ��
        lastPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision != null && collision.gameObject.TryGetComponent(out Entity entity))
        {
            if (entity.flag == flag)
                return;
        }

        

        if (trailPrefab)
        {
            trailPrefab.transform.parent = null;

            var particleSystems = trailPrefab.GetComponentsInChildren<ParticleSystem>();
            foreach (var particle in particleSystems)
            {
                var main = particle.main;
                main.loop = false;
            }
        }

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void DelayTrail()
    {
        trailPrefab.SetActive(true);
    }    
}
