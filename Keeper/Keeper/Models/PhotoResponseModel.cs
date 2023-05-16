using System;
using System.IO;

namespace Keeper.Models;
public class PhotoResponseModel
{
    public MemoryStream PhotoFileStream { get; set; }

    public PhotoResponseModel(MemoryStream photoFileStream)
    {
        PhotoFileStream = photoFileStream;
    }

}
