using UnityEngine;

public interface IBoardLayout
{   
    Vector2 Padding { get; }
    float SlotSpacing { get; }
    float Tx { get; }
    int MaxSupplySlotCount { get; }
}

public class BoardLayout : IBoardLayout
{
    private readonly IBoardModel model;
    private Vector2? padding;
    private float? tx, ty;

    private BoardLayout(IBoardModel model)
    {
        this.model = model;
    }
    
    public Vector2 Padding
    {
        get
        {
            if (padding.HasValue)
                return padding.Value;

            padding = model.MinPadding + Tx * model.FlexiblePadding.x * Vector2.right + Ty * model.FlexiblePadding.y * Vector2.up;

            return padding.Value;
        }
    }

    public float SlotSpacing => model.MinSlotSpacing + model.FlexibleSlotSpacing * Tx;

    public float Tx
    {
        get
        {
            if (tx.HasValue)
                return tx.Value;
            
            var refRatio = model.ReferenceAspectRatio.x / model.ReferenceAspectRatio.y;
            var wideRatio = model.WidestAspectRatio.x / model.WidestAspectRatio.y;
            var widthRatio = Mathf.Clamp((float) Screen.width / Screen.height, refRatio, wideRatio);
            
            tx = (widthRatio - refRatio) / (wideRatio - refRatio);

            return tx.Value;
        }
    }

    public int MaxSupplySlotCount => model.MaxSupplySlotCount;
    
    private float Ty
    {
        get
        {
            if (ty.HasValue)
                return ty.Value;
            
            var refRatio = model.ReferenceAspectRatio.y / model.ReferenceAspectRatio.x;
            var tallRatio = model.TallestAspectRatio.y / model.TallestAspectRatio.x;
            var heightRatio = Mathf.Clamp((float) Screen.height / Screen.width, refRatio, tallRatio);
            
            ty = (heightRatio - refRatio) / (tallRatio - refRatio);

            return ty.Value;
        }
    }    
}