using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]

public class HexGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    
    public HexCell cellPrefab;
    private HexCell[] cells;
    
    public Text cellLabelPrefab;
    private Canvas gridCanvas;
    
    private HexMesh hexMesh;
    
    public Color defaultColor = Color.white;
    public Color touchedColor = Color.magenta;
    void Awake () {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();
        cells = new HexCell[height * width];

        for (int z = 0, i = 0; z < height; z++) {
            for (int x = 0; x < width; x++) {
                CreateCell(x, z, i++);
            }
        }
    }

    private void Start()
    {
        hexMesh.Triangulate(cells);
    }
    void CreateCell (int x, int z, int i) {
        Vector3 position;
        position.x = (x + z%2 * 0.5f) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate(cellPrefab);
        cell.transform.SetParent(hexMesh.transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;
        
        Text label = Instantiate(cellLabelPrefab, gridCanvas.transform, false);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
    }
    public void ColorCell (Vector3 position, Color color) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        HexCell cell = cells[index];
        cell.color = color;
        hexMesh.Triangulate(cells);
    }
}
