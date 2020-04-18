using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public static List<Box> allBoxes = new List<Box>();

    public Vector3 size = new Vector3(1,1,1);
    
    [Range(0.01f, 0.1f)]
    public float wallThickness = 0.025f;

    GameObject front, back, left, right, bottom, topFront, topBack, topLeft, topRight;
    GameObject topFrontParent, topBackParent, topLeftParent, topRightParent ;

    public Material material;
    public Material flapMaterial;

    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;

    public Material adulatedRedMaterial;
    public Material adulatedGreenMaterial;
    public Material adulatedBlueMaterial;

    public bool open = false;

    float openAnimationTime = 0f;

    [Range(0.1f, 10.0f)]
    public float openAnimationTimeLength = 1f;

    public AnimationCurve openAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);

    public bool onConveyor = false;
    public GameObject boxLabel;

    public GameObject detector;

    public Task Task
    {
        get
        {
            return boxLabel.GetComponent<BoxLabel>().task;
        }
    }

    public void UpdateAdulatedVisuals()
    {
        switch (Task.sectorColor)
        {
            case ConveyorSectorColor.red:
                SetMaterial(adulatedRedMaterial);
                break;
            case ConveyorSectorColor.green:
                SetMaterial(adulatedGreenMaterial);
                break;
            case ConveyorSectorColor.blue:
                SetMaterial(adulatedBlueMaterial);
                break;
            default:
                Task.sorted = false;
                SetMaterial(material);
                break;
        }
    }

    public void MarkAsSorted()
    {
        Task.sorted = true;
        switch (Task.sectorColor)
        {
            case ConveyorSectorColor.red:
                SetMaterial(redMaterial);
                break;
            case ConveyorSectorColor.green:
                SetMaterial(greenMaterial);
                break;
            case ConveyorSectorColor.blue:
                SetMaterial(blueMaterial);
                break;
            default:
                Task.sorted = false;
                SetMaterial(material);
                break;
        }
    }

    void SetMaterial(Material mat)
    {
        foreach (var obj in new List<GameObject> { front, back, left, right, bottom })
        {
            obj.GetComponent<MeshRenderer>().material = mat;
        }

        foreach (var obj in new List<GameObject> { topFront, topBack, topLeft, topRight })
        {
            obj.GetComponent<MeshRenderer>().material = mat;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        front = GameObject.CreatePrimitive(PrimitiveType.Cube);
        back = GameObject.CreatePrimitive(PrimitiveType.Cube);
        left = GameObject.CreatePrimitive(PrimitiveType.Cube);
        right = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topFront = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topRight = GameObject.CreatePrimitive(PrimitiveType.Cube);

        topFrontParent = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topBackParent = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topLeftParent = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topRightParent = GameObject.CreatePrimitive(PrimitiveType.Cube);

        front.transform.parent = transform;
        back.transform.parent = transform;
        left.transform.parent = transform;
        right.transform.parent = transform;
        bottom.transform.parent = transform;

        topFrontParent.transform.parent = transform;
        topBackParent.transform.parent = transform;
        topLeftParent.transform.parent = transform;
        topRightParent.transform.parent = transform;


        topFront.transform.parent = topFrontParent.transform;
        topFront.transform.localScale = new Vector3(1, 1, 0.5f);
        topFront.transform.localPosition = new Vector3(0, 0, -0.25f);

        topBack.transform.parent = topBackParent.transform;
        topBack.transform.localScale = new Vector3(1, 1, 0.5f);
        topBack.transform.localPosition = new Vector3(0, 0, 0.25f);

        topLeft.transform.parent = topLeftParent.transform;
        topLeft.transform.localScale = new Vector3(0.5f, 1, 1);
        topLeft.transform.localPosition = new Vector3(-0.25f, 0, 0);

        topRight.transform.parent = topRightParent.transform;
        topRight.transform.localScale = new Vector3(0.5f, 1, 1);
        topRight.transform.localPosition = new Vector3(0.25f, 0, 0);

        foreach (var obj in new List<GameObject> {topFrontParent, topBackParent, topLeftParent, topRightParent, topFrontParent, topBackParent, topLeftParent, topRightParent} ) {
            obj.GetComponent<BoxCollider>().enabled = false;
            obj.GetComponent<MeshRenderer>().enabled = false;
        }

        foreach (var obj in new List<GameObject> {front, back, left, right, bottom} ) {
            Destroy(obj.GetComponent<BoxCollider>());
        }

        foreach (var obj in new List<GameObject> { topFront, topBack, topLeft, topRight })
        {
            Destroy(obj.GetComponent<BoxCollider>());
        }

        SetMaterial(material);

        boxLabel.transform.parent = transform;
        boxLabel.GetComponent<BoxLabel>().Box = this;
        allBoxes.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (open) {
            openAnimationTime = Mathf.Max(openAnimationTime + (Time.deltaTime / openAnimationTimeLength), 0);
        } else {
            openAnimationTime = Mathf.Min(openAnimationTime - (Time.deltaTime / openAnimationTimeLength), 1);
        }


        var openness = openAnimationCurve.Evaluate(openAnimationTime);
        
        front.transform.localPosition = new Vector3(0, 0, (0.5f * size.z) - wallThickness/2f);
        front.transform.localScale = new Vector3(size.x, size.y, wallThickness);

        back.transform.localPosition = new Vector3(0, 0, (-0.5f * size.z) + wallThickness/2f);
        back.transform.localScale = new Vector3(size.x, size.y, wallThickness);

        left.transform.localPosition = new Vector3((0.5f * size.x) - wallThickness/2f, 0, 0);
        left.transform.localScale = new Vector3(wallThickness, size.y, size.z);

        right.transform.localPosition = new Vector3((-0.5f * size.x) + wallThickness/2f, 0, 0);
        right.transform.localScale = new Vector3(wallThickness, size.y, size.z);

        bottom.transform.localPosition = new Vector3(0, -0.5f * size.y, 0);
        bottom.transform.localScale = new Vector3(size.x, wallThickness, size.z);

        var topZAngle = openness * 260.0f;
        topFrontParent.transform.localPosition = new Vector3(0, (0.5f * size.y), 0.5f * size.z);
        topFrontParent.transform.localScale = new Vector3(size.x, wallThickness, size.z);
        topFrontParent.transform.localEulerAngles = new Vector3(topZAngle, 0, 0);

        topBackParent.transform.localPosition = new Vector3(0, (0.5f * size.y), -0.5f * size.z);
        topBackParent.transform.localScale = new Vector3(size.x, wallThickness, size.z);
        topBackParent.transform.localEulerAngles = new Vector3(-topZAngle, 0, 0);

        var topXAngle = ((Mathf.Min(Mathf.Max(openness, 0.24f), 1f) - 0.25f) / 0.75f) * -260.0f;
        topLeftParent.transform.localPosition = new Vector3(0.5f * size.x, (0.5f * size.y), 0);
        topLeftParent.transform.localScale = new Vector3((0.5f * size.x) + wallThickness, wallThickness, (1f * size.z) - wallThickness);
        topLeftParent.transform.localEulerAngles = new Vector3(0, 0, topXAngle);

        topRightParent.transform.localPosition = new Vector3(-0.5f * size.x, (0.5f * size.y), 0);
        topRightParent.transform.localScale = new Vector3((0.5f * size.x) + wallThickness, wallThickness, (1f * size.z) - wallThickness);
        topRightParent.transform.localEulerAngles = new Vector3(0, 0, -topXAngle);

        this.GetComponent<BoxCollider>().size = new Vector3(size.x + (2*wallThickness), size.y + (2*wallThickness), size.z + (2*wallThickness));


        boxLabel.transform.localPosition = new Vector3((0.5f * size.x) - 0.25f, (0.5f * size.y) - 0.30f, (0.5f * size.z) + wallThickness/2f);



        detector.transform.localPosition = new Vector3(0, 0.52f * size.y, 0);
        detector.transform.localScale = new Vector3(size.x, wallThickness, size.z);
    }

    private void OnDestroy()
    {
        allBoxes.Remove(this);
    }
}