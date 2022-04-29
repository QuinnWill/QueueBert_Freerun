using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.InputSystem;

public class ItemSwitchUI : MonoBehaviour
{

    [SerializeField]
    private Image selectionImage;

    [SerializeField]
    private Sprite nullSprite;

    [SerializeField]
    ItemManager itemManager;

    [SerializeField]
    private List<CinemachineInputProvider> cameraMovementComponents;

    public int maxItems;

    [Header("Animate UI")]

    [SerializeField]
    private AnimationCurve scaleCurve;

    [SerializeField]
    private float scaleTime = 0.25f;

    [SerializeField]
    private float itemRadius = 1f;

    private float curveValue;


    [Header("Time Controls")]

    [SerializeField]
    private float timeDilationAmount = 4;

    public bool switching { get; private set; }
    private bool primarySwitch;

    private float originalTimeScale;

    private Vector2 choiceVector;

    private void OnEnable()
    {
        InputEventManager.selectItemStart += StartSelect;
        InputEventManager.selectItemEnd += EndSelect;
        InputEventManager.cameraDelta += OnMouseDelta;
    }

    private void OnDisable()
    {
        InputEventManager.selectItemStart -= StartSelect;
        InputEventManager.selectItemEnd -= EndSelect;
        InputEventManager.cameraDelta -= OnMouseDelta;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (selectionImage == null)
        {
            selectionImage = GetComponentInChildren<Image>();
        }
        if (scaleCurve == null)
        {
            scaleCurve = new AnimationCurve();
        }

        

    }

    // Update is called once per frame
    void Update()
    {
        if (switching && curveValue < 1)
        {
            curveValue += Time.deltaTime * timeDilationAmount / scaleTime;

            selectionImage.transform.localScale = Vector3.one * scaleCurve.Evaluate(curveValue);
        }
        else if (!switching && curveValue > 0)
        {
            curveValue -= Time.deltaTime * timeDilationAmount / scaleTime;

            selectionImage.transform.localScale = Vector3.one * scaleCurve.Evaluate(curveValue);


        }
        else if(curveValue <= 0)
        {
            selectionImage.gameObject.SetActive(false);
        }

    }

    private void OnMouseDelta(Vector2 delta)
    {
        choiceVector = (choiceVector + delta).normalized;
    }



    private void StartSelect(bool isPrimary)
    {
        if (!switching)
        {
            choiceVector = Vector2.zero;
            primarySwitch = isPrimary;
            switching = true;
            originalTimeScale = Time.timeScale;
            Time.timeScale /= timeDilationAmount;

            selectionImage.transform.localScale = Vector3.one * 0.01f;
            selectionImage.gameObject.SetActive(true);
            curveValue = 0;
            List<UsableItem> items = new List<UsableItem>();

            items.AddRange(itemManager.getInactive());

            items.Add(null);

            Debug.Log("Items: " + items);

            for (int i = 0; i < items.Count; i++)
            {
                Debug.Log(items[i]);
                GameObject itemImage = new GameObject("Image");
                itemImage.transform.parent = this.transform;
                itemImage.transform.position = transform.position;
                itemImage.transform.localScale /= 2.5f;
                Image image = itemImage.AddComponent<Image>();
                image.color = Color.black;

                if (items[i] != null)
                {
                    image.sprite = items[i].UISprite;
                }
                else
                {
                    image.sprite = nullSprite;
                }

                float itemAngle = Mathf.PI * 2 / items.Count * (items.Count -1 - i) - Mathf.PI * 0.5f;

                Vector3 itemVector = new Vector3(Mathf.Cos(itemAngle), Mathf.Sin(itemAngle));
                image.transform.position += itemVector * itemRadius;
            }

            if (cameraMovementComponents.Count > 0)
            {
                for (int i = 0; i < cameraMovementComponents.Count; i++)
                {
                    cameraMovementComponents[i].enabled = false;
                    
                }
            }
        }
    }

    private void EndSelect(bool isPrimary)
    {
        if (isPrimary == primarySwitch)
        {
            
            switching = false;
            Time.timeScale = originalTimeScale;
            

            UsableItem chosenItem = null;

            float leastDistance = 100;

            List<UsableItem> items = new List<UsableItem>();

            items.AddRange(itemManager.getInactive());

            items.Add(null);

            for (int i = 0; i < items.Count; i++)
            {
                float itemAngle = Mathf.PI * 2 / items.Count * (items.Count - 1 - i) - Mathf.PI * 0.5f;
                Vector2 itemVector = new Vector2(Mathf.Cos(itemAngle), Mathf.Sin(itemAngle));
                if (Vector2.Distance(itemVector.normalized, choiceVector) < leastDistance)
                {
                    leastDistance = Vector2.Distance(itemVector, choiceVector);
                    chosenItem = items[i];
                }
            }

            Image[] images = GetComponentsInChildren<Image>();

            foreach(Image pic in images)
            {
                if (pic.tag == "Untagged")
                {
                    Destroy(pic.gameObject);
                }
            }

            Debug.Log("Chose: " + chosenItem);

            itemManager.SwitchActive(chosenItem, isPrimary);

            if (cameraMovementComponents.Count > 0)
            {
                for (int i = 0; i < cameraMovementComponents.Count; i++)
                {
                    cameraMovementComponents[i].enabled = true;
                }
            }
        }
    }
}
