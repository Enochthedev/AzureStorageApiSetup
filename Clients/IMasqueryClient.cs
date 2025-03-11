using System;
using System.Threading.Tasks;

public interface IMasqueryClient
{
    Task<bool> CreateUserFileRecordAsync(Guid userId, string company, string category, string fileUrl, string relativePath);
}