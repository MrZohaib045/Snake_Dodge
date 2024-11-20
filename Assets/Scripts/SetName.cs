using UnityEngine;
using TMPro;

public class SetName : MonoBehaviour
{
    public GameObject textPrefab;
    private TextMeshPro text;
    public Vector2 name_pos;
    public string Name;

    public void Start()
    {
        GameObject textInstance = Instantiate(textPrefab, transform.position, Quaternion.identity);
        textInstance.transform.SetParent(transform.parent);
        text = textInstance.GetComponent<TextMeshPro>();
        Name = transform.parent.name;
        name_pos = new(-1.25f, 0);
        text.text = Name;
    }

    public void Update()
    {
        if (text != null)
        {
            text.transform.position = transform.position + new Vector3(name_pos.x, name_pos.y, 0f);
        }
    }
}
