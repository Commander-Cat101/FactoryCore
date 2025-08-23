using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace FactoryCore.UI
{
    [RegisterTypeInIl2Cpp]
    internal class LineBetweenObjects : MonoBehaviour
    {
        Transform obj1, obj2;
        public static GameObject Create(Color color, Transform obj1, Transform obj2, Transform parent)
        {
            var obj = new GameObject("Line");
            obj.transform.SetParent(parent);
            obj.transform.localScale = Vector3.one;

            var canvas = obj.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 5;

            var image = obj.gameObject.AddComponent<Image>();
            image.color = color;

            var comp = obj.AddComponent<LineBetweenObjects>();
            comp.obj1 = obj1;
            comp.obj2 = obj2;
            return obj;
        }

        public void Update()
        {
            transform.position = (obj1.transform.position + obj2.position) / 2;
            Vector3 dif = transform.position - obj2.position;

            GetComponent<RectTransform>().sizeDelta = new Vector3(dif.magnitude * 4.75f, 25);
            GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
        }
    }
}
