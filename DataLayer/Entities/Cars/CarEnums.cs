
namespace DataLayer.Entities.Cars
{
    public enum CarDocumentType
    {
        STK = 0,
        TechnicalCertificate = 1,
    }

    public enum CarServiceState
    {
        Fine = 0,
        NeedStk = 1,
        NeedOilChange = 2,
        SoftBroken = 3,
        HardBroken = 4,
        NeedsToll = 5,
    }

    public enum CarState
    {
        Parked = 0,
        NeedsService = 1,
        Borrowed = 2,
        Lost = 3,
        InService = 4,
        MissingDocuments = 5,
    }

}
