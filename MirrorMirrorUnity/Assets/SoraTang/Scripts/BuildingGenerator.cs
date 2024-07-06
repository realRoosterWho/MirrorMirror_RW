using UnityEngine;

[ExecuteInEditMode]
public class BuildingGenerator : MonoBehaviour
{
    public float width = 10f;       // 建筑宽度
    public float length = 10f;      // 建筑长度
    public float height = 5f;       // 建筑高度
    public float wallThickness = 0.5f;  // 墙壁厚度

    public void GenerateBuilding()
    {
        // 删除现有子对象
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        // 生成地板
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.transform.position = new Vector3(0, -wallThickness / 2, 0);
        floor.transform.localScale = new Vector3(width, wallThickness, length);
        floor.transform.parent = this.transform;

        // 生成墙壁
        CreateWall(new Vector3((width + wallThickness) / 2, height / 2, 0), new Vector3(wallThickness, height, length + wallThickness));
        CreateWall(new Vector3(-(width + wallThickness) / 2, height / 2, 0), new Vector3(wallThickness, height, length + wallThickness));
        CreateWall(new Vector3(0, height / 2, (length + wallThickness) / 2), new Vector3(width, height, wallThickness));
        CreateWall(new Vector3(0, height / 2, -(length + wallThickness) / 2), new Vector3(width, height, wallThickness));
    }

    void CreateWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = position;
        wall.transform.localScale = scale;
        wall.transform.parent = this.transform;
    }
}
