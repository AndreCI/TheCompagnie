using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
public class CursorManager : MonoBehaviour
{

    private static CursorManager instance;
    public static CursorManager Instance { get => instance; }

    public enum CURSOR_TYPE { DEFAULT, GRAB, PREGRAB, BOOK, ATTACK};
    private CURSOR_TYPE type_ = CURSOR_TYPE.DEFAULT;
    public CURSOR_TYPE type { get
        {
            return type_;
        } set
        {
            SetCursor(value);
            type_ = value;
        }
    }
    private void SetCursor(CURSOR_TYPE type_)
    {
        int i = 0;
        
        foreach(CURSOR_TYPE pp in Enum.GetValues(typeof(CURSOR_TYPE)))
        {
            if(type_ == pp)
            {
                if (i != 1 && i != 2)
                {
                    Cursor.SetCursor(cursors[i], new Vector2(cursors[i].width / 3f, 0f), CursorMode.ForceSoftware);
                }
                else
                {
                    Cursor.SetCursor(cursors[i], new Vector2(cursors[i].width / 2f, cursors[i].height / 2f), CursorMode.ForceSoftware);
                }                
            }
            i++;
        }
        
    }
    public List<Texture2D> cursors;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        type = CURSOR_TYPE.DEFAULT;
    }
}
     