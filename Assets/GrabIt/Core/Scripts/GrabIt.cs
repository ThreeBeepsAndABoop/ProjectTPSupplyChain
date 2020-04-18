using System.Collections;
using UnityEngine;

namespace Lightbug.GrabIt
{

    [System.Serializable]
    public class GrabObjectProperties
    {

        public bool m_useGravity = false;
        public float m_drag = 10;
        public float m_angularDrag = 10;
        public RigidbodyConstraints m_constraints = RigidbodyConstraints.FreezeRotation;

    }

    public class GrabIt : MonoBehaviour
    {

        [Header("Input")]
        [SerializeField] KeyCode m_rotatePitchPosKey = KeyCode.I;
        [SerializeField] KeyCode m_rotatePitchNegKey = KeyCode.K;
        [SerializeField] KeyCode m_rotateYawPosKey = KeyCode.L;
        [SerializeField] KeyCode m_rotateYawNegKey = KeyCode.J;

        [Header("Grab properties")]

        [SerializeField]
        [Range(4, 50)]
        float m_grabSpeed = 12;

        [SerializeField]
        [Range(0.1f, 5)]
        float m_grabMinDistance = 1;

        [SerializeField]
        [Range(4, 25)]
        float m_grabMaxDistance = 4;

        [SerializeField]
        [Range(1, 10)]
        float m_scrollWheelSpeed = 5;

        [SerializeField]
        [Range(50, 500)]
        float m_angularSpeed = 300;

        [SerializeField]
        [Range(10, 50)]
        float m_impulseMagnitude = 25;




        [Header("Affected Rigidbody Properties")]
        [SerializeField] GrabObjectProperties m_grabProperties = new GrabObjectProperties();

        GrabObjectProperties m_defaultProperties = new GrabObjectProperties();

        [Header("Layers")]
        [SerializeField]
        LayerMask m_collisionMask;



        Rigidbody m_targetRB = null;
        int m_targetLayer;
        Transform m_transform;

        Quaternion m_targetRot;

        Vector3 m_targetPos;
        GameObject m_hitPointObject;
        float m_targetDistance;

        bool m_grabbing = false;
        bool m_applyImpulse = false;
        bool m_isHingeJoint = false;

        Grabbable grabbed;


        public GameObject GrabbedObject
        {
            get
            {
                if(m_targetRB == null) { return null; } 
                return m_targetRB.gameObject;
            }
        }

        //Debug
        LineRenderer m_lineRenderer;



        void Awake()
        {
            m_transform = transform;
            m_hitPointObject = new GameObject("Point");

            m_lineRenderer = GetComponent<LineRenderer>();

            m_collisionMask = ~(1 << 10);

        }

        void Update()
        {
            if (m_grabbing)
            {

                m_targetDistance += Input.GetAxisRaw("Mouse ScrollWheel") * m_scrollWheelSpeed;
                m_targetDistance = Mathf.Clamp(m_targetDistance, m_grabMinDistance, m_grabMaxDistance);

                m_targetPos = m_transform.position + m_transform.forward * m_targetDistance;

                //if (!m_isHingeJoint)
                //{
                //    if (Input.GetKey(m_rotatePitchPosKey) || Input.GetKey(m_rotatePitchNegKey) || Input.GetKey(m_rotateYawPosKey) || Input.GetKey(m_rotateYawNegKey))
                //    {
                //        m_targetRB.constraints = RigidbodyConstraints.None;
                //    }
                //    else
                //    {
                //        m_targetRB.constraints = m_grabProperties.m_constraints;
                //    }
                //}

            }

        }

        public void YeetGrabbed()
        {
            Yeet(grabbed);
        }

        public void Yeet(Grabbable grabbable)
        {
            if(grabbed != grabbable) { return; }
            m_applyImpulse = true;
        }

        public void Release(Grabbable grabbable)
        {
            if (grabbed != grabbable) { return; }
            Reset();
            m_grabbing = false;
        }

        public void ReleaseGrabbed()
        {
            Release(grabbed);
        }

        public void Grab(RaycastHit hitInfo, Grabbable grabbable)
        {
            if(grabbed == grabbable)
            {
                Release(grabbable);
            }

            Rigidbody rb = grabbable.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Set(rb, hitInfo.distance);
                m_grabbing = true;
            }
        }

        void Set(Rigidbody target, float distance)
        {
            m_targetRB = target;
            m_isHingeJoint = target.GetComponent<HingeJoint>() != null;

            m_targetLayer = m_targetRB.gameObject.layer;
            m_targetRB.gameObject.layer = 13;

            grabbed = m_targetRB.GetComponent<Grabbable>();
            grabbed.Grabbed = true;

            //Rigidbody default properties	
            m_defaultProperties.m_useGravity = m_targetRB.useGravity;
            m_defaultProperties.m_drag = m_targetRB.drag;
            m_defaultProperties.m_angularDrag = m_targetRB.angularDrag;
            m_defaultProperties.m_constraints = m_targetRB.constraints;

            //Grab Properties	
            m_targetRB.useGravity = m_grabProperties.m_useGravity;
            m_targetRB.drag = m_grabProperties.m_drag;
            m_targetRB.angularDrag = m_grabProperties.m_angularDrag;
            m_targetRB.constraints = m_isHingeJoint ? RigidbodyConstraints.None : m_grabProperties.m_constraints;

            Vector3 eulerAngles = transform.eulerAngles;
            m_targetRot = Quaternion.Euler(0, 180 + eulerAngles.y, 0);

            const float speed = 360.0f;
            var a = Quaternion.Angle(m_targetRB.transform.rotation, m_targetRot); //degrees we must travel
            StartCoroutine(RotateOverTime(m_targetRB.transform.rotation, m_targetRot, a / speed));

            m_hitPointObject.transform.SetParent(target.transform);

            m_targetDistance = distance;
            m_targetPos = m_transform.position + m_transform.forward * m_targetDistance;

            m_hitPointObject.transform.position = m_targetPos;
            m_hitPointObject.transform.LookAt(m_transform);

        }

        void Reset()
        {
            //Grab Properties	
            m_targetRB.useGravity = m_defaultProperties.m_useGravity;
            m_targetRB.drag = m_defaultProperties.m_drag;
            m_targetRB.angularDrag = m_defaultProperties.m_angularDrag;
            m_targetRB.constraints = m_defaultProperties.m_constraints;

            m_targetRB.gameObject.layer = m_targetLayer;
            m_targetRB = null;

            grabbed.Grabbed = false;
            grabbed = null;

            m_hitPointObject.transform.SetParent(null);

            if (m_lineRenderer != null)
                m_lineRenderer.enabled = false;
        }

        IEnumerator RotateOverTime(Quaternion originalRotation, Quaternion finalRotation, float duration)
        {
            if (duration > 0f)
            {
                float startTime = Time.time;
                float endTime = startTime + duration;
                m_targetRB.transform.rotation = originalRotation;
                yield return null;
                while (Time.time < endTime && m_grabbing)
                {
                    float progress = (Time.time - startTime) / duration;
                    // progress will equal 0 at startTime, 1 at endTime.
                    m_targetRB.transform.rotation = Quaternion.Slerp(originalRotation, finalRotation, progress);
                    yield return null;
                }
            }

            if (m_targetRB)
            {
                m_targetRB.transform.rotation = finalRotation;
            }
        }

        void Grab()
        {
            Vector3 hitPointPos = m_hitPointObject.transform.position;
            Vector3 dif = m_targetPos - hitPointPos;

            if (m_isHingeJoint)
                m_targetRB.AddForceAtPosition(m_grabSpeed * dif * 100, hitPointPos, ForceMode.Force);
            else
                m_targetRB.velocity = m_grabSpeed * dif;

            if (m_lineRenderer != null)
            {
                m_lineRenderer.enabled = true;
                m_lineRenderer.SetPositions(new Vector3[] { m_targetPos, hitPointPos });
            }
        }

        void Rotate()
        {
            if (Input.GetKey(m_rotatePitchPosKey))
            {
                m_targetRB.AddTorque(m_transform.right * m_angularSpeed);
            }
            else if (Input.GetKey(m_rotatePitchNegKey))
            {
                m_targetRB.AddTorque(-m_transform.right * m_angularSpeed);
            }

            if (Input.GetKey(m_rotateYawPosKey))
            {
                m_targetRB.AddTorque(-m_transform.up * m_angularSpeed);
            }
            else if (Input.GetKey(m_rotateYawNegKey))
            {
                m_targetRB.AddTorque(m_transform.up * m_angularSpeed);
            }
        }

        void FixedUpdate()
        {
            if (!m_grabbing)
                return;

            //if (!m_isHingeJoint)
            //    Rotate();

            Grab();

            if (m_applyImpulse)
            {
                m_targetRB.velocity = m_transform.forward * m_impulseMagnitude;
                Reset();
                m_grabbing = false;
                m_applyImpulse = false;
            }

        }

    }

}
