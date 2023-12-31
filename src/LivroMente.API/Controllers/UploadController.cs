using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Google.Apis.Drive.v3.DriveService;

namespace LivroMente.API.Controllers
{
    [Route("Api/Controller/[action]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        //  static string[] Scopes = { DriveService.ScopeConstants.DriveFile};//DriveService.Scope.DriveFile };
        //  static string ApplicationName = "LivroEMente"; // Substitua pelo nome da sua aplicação
        //  static string ServiceAccountKeyPath = "client_secret.json"; // Substitua pelo caminho para o arquivo JSON de chave de serviço

        // static void Main(string[] args)
        // {
        //     var credential = GetServiceAccountCredential();

        //     // Crie um serviço Drive autenticado
        //     var service = new DriveService(new BaseClientService.Initializer()
        //     {
        //         HttpClientInitializer = credential,
        //         ApplicationName = ApplicationName,
        //     });

        //     // Agora você pode usar 'service' para fazer chamadas à API do Google Drive
        // }

        // static ServiceAccountCredential GetServiceAccountCredential()
        // {
        //     using (var stream = new FileStream(ServiceAccountKeyPath, FileMode.Open, FileAccess.Read))
        //     {
        //         var credential = GoogleCredential.FromStream(stream)
        //                                         .CreateScoped(Scopes)
        //                                         .UnderlyingCredential as ServiceAccountCredential;

        //         if (credential != null)
        //         {
        //             return credential;
        //         }
        //         else
        //         {
        //             throw new Exception("Falha ao criar credencial do serviço.");
        //         }
        //     }
        // }
    
        private static DriveService GetService()
        {
            var tokenResponse = new TokenResponse
            {
                AccessToken =
                "ya29.a0AfB_byDt6nJ3oIyVxQl3ESlBNkPjEQtb0UJL8fcqnl2dGw3kheBLRqpNzvc99sYT_Nu8IAnfd4C430fYk9kEv7Rf4X_OkbVEgaUQTD6MMf-SU8LNbOstAtL2LBoi0OdnbTjpkj-xOFCxNF_kp0JTBBTy2GvHLdJu2hAFaCgYKATYSARASFQHGX2Midt1GztSKPikuMapdlW0b1A0171",
                
                RefreshToken = "1//04M7WVkEcM5oECgYIARAAGAQSNwF-L9IraGX18C_CjP6JmC3JkMpt0BUqTUaepGhuegGWDeynEWOibgQ1vrcU89_Czgr74TqGQx8",
            };
            
            var applicationName = "LivroEMente" ;// Use the name of the project in Google Cloud
            var username = "grappeincvinum@gmail.com"; // Use your email
            
            var apiCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = "790068342173-q0bqobbsdblgki94pvm5888e8dp4aqnb.apps.googleusercontent.com",
                    ClientSecret = "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"
                },
                Scopes = new[] { Scope.Drive },
                DataStore = new FileDataStore(applicationName)
            });
    
            var credential = new UserCredential(apiCodeFlow, username, tokenResponse); 
            
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });
            
            return service;
        }
        
         [HttpPost]
         [AllowAnonymous] 
        public IActionResult Upload(IFormFile arquivo)
        {
            if (arquivo != null && arquivo.Length > 0)
            {
                using (var stream = arquivo.OpenReadStream())
                {
                    var link = UploadFile(stream, arquivo.FileName, arquivo.ContentType, "1m1yulxFuOKXx3Pc6BkAXkAgwcv_8qp6g", "Descrição do arquivo");
                 
                    return Ok(link);
                }
            }
            else
            {
                return BadRequest("Nenhum arquivo enviado.");
            }
        }




        private string UploadFile(Stream file, string fileName, string fileMime, string folder, string fileDescription)
        {


            DriveService service = GetService();
        
            var driveFile = new Google.Apis.Drive.v3.Data.File();
            driveFile.Name = fileName;
            driveFile.Description = fileDescription;
            driveFile.MimeType = fileMime;
            driveFile.Parents = new string[] { folder };
            
            var request = service.Files.Create(driveFile, file, fileMime);
            request.Fields = "id";
            
            var response = request.Upload();
            if (response.Status != Google.Apis.Upload.UploadStatus.Completed)
                throw response.Exception;

            System.Threading.Thread.Sleep(5000); // Aguarde 5 segundos antes de pegar id
            var fileId = request.ResponseBody.Id;
            var link = $"https://drive.google.com/uc?id={fileId}";
            return link;
        }
        
        [HttpDelete]
         [AllowAnonymous] 
        public void DeleteFile(string fileId)
        {
            var service = GetService();
            var command = service.Files.Delete(fileId);
            var result = command.Execute();
        }

        // [HttpGet]
        //  [AllowAnonymous] 
        // public IEnumerable<Google.Apis.Drive.v3.Data.File> GetFiles(string folder)
        // {
        //     var service = GetService();
            
        //     var fileList = service.Files.List();
        //     fileList.Q =$"mimeType!='application/vnd.google-apps.folder' and '{folder}' in parents";
        //     fileList.Fields = "nextPageToken, files(id, name, size, mimeType)";
            
        //     var result = new List<Google.Apis.Drive.v3.Data.File>();
        //     string pageToken = null;
        //     do
        //     {
        //         fileList.PageToken = pageToken;
        //         var filesResult = fileList.Execute();
        //         var files = filesResult.Files;
        //         pageToken = filesResult.NextPageToken;
        //         result.AddRange(files);
        //     } while (pageToken != null);
            
        //     return result;
        // }

    }
}