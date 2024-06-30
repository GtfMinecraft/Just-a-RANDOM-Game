using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WheelUI : MonoBehaviour
{
    protected int GetSection(float startAngle, float anglePerSection, int sectionCount, float freeDistance)
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            Vector2 startDirection = Quaternion.AngleAxis(startAngle, Vector3.forward) * Vector2.right;
            Vector2 mouseDirection = new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2);
            if (mouseDirection.magnitude < freeDistance)
            {
                return -1;
            }
            int section;
            if (startDirection.x * mouseDirection.y - startDirection.y * mouseDirection.x >= 0)
            {
                section = (int)(Vector2.Angle(startDirection, mouseDirection) / anglePerSection);
            }
            else
            {
                section = (int)((360f - Vector2.Angle(startDirection, mouseDirection)) / anglePerSection);
            }

            if(section >= sectionCount)
            {
                return -1;
            }
            
            return section;
        }
        return -1;
    }
}
