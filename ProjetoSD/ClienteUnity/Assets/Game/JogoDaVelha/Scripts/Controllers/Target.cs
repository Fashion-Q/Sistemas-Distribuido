using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private int x, y;
    [SerializeField] private GameController gameController;
    public void TargetPressed()
    {
        gameController.Jogar(x,y);
    }

    private void OnMouseDown()
    {
        TargetPressed();
    }
}
