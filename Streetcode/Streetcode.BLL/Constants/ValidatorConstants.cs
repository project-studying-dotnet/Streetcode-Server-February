namespace Streetcode.BLL.Constants;

public static class ValidatorConstants
{
    public const int PositionMinLength = 2;

    public const int PositionMaxLength = 50;

    public const int RelatedTermWordMaxLength = 30;

    public const int TitleMaxLength = 100;

    public const int DescriptionMaxLength = 500;

    public const int UrlTitleMaxLength = 200;

    public const int ImageAltMaxLength = 200;

    public const string MimeTypeRegularExpression = @"^[\w\-]+\/[\w\-]+$";

    public const string ExtensionRegularExpression = @"^[a-zA-Z]+$";

    public const int EmailFromMaxLength = 2;

    public const int EmailContentMaxLength = 500;
}
