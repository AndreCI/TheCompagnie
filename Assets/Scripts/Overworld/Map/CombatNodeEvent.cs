using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

[Serializable]
public class CombatNodeEvent : NodeEvent
{
    public List<Enemy> enemies;

    public override void Perform()
    {
        SceneManager.LoadScene(1);
    }
}
