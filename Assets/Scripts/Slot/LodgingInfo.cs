public struct LodgingInfo
{
    public LodgingInfo(
        ICardBond bond,
        ArrangementInfo arrangementInfo
    )
    {
        Bond = bond;
        ArrangementInfo = arrangementInfo;
    }
    
    public ICardBond Bond { get; }
    public ArrangementInfo ArrangementInfo { get; }
}