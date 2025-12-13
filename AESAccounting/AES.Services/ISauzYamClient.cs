using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES.Services;
public class SauzYamResponse
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}

public interface ISauzYamClient
{
    Task<SauzYamResponse> RegisterMaterialAsync(string materialNumber, decimal massKg, decimal enrichment);
}
