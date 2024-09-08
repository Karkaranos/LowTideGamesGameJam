using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class buttondisappear : MonoBehaviour
{
    [SerializeField] public Image goop;

}

public interface IPointerEnterHandler : IEventSystemHandler
{
  //List<GameObject> GetPointers();
  //List<GameObject> hovered;
  //List<GameObject> buttons GetButtons()
  //private Image goop;
    void OnPointerExit(PointerEventData eventData)
    {
      //eventData.button.CompareTo();
      //eventData.hovered.Find().gameObject.SetActive(false);
    }



    public GameObject pointerEnter { get; set; }
}

public interface IpointerExitHandler : IEventSystemHandler 
{
    void OnPointerExit(PointerEventData eventData);

    public GameObject pointerExit { get; set; }

}

public abstract class UIBehaviour : MonoBehaviour
{

}

