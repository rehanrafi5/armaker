namespace ARMarker
{ 

    public enum ARStatus
    {

        UNSET = -1,

        ScanningMarker = 0,
        MarkerDetected = 1,
        ActivelyTrackingMarker = 2,
        LostMarker = 4,

        SessionOriginCreated = 8,

    }

}