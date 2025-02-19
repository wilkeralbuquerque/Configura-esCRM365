﻿using ceTe.DynamicPDF;
using ceTe.DynamicPDF.Conversion;
using ceTe.DynamicPDF.PageElements;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using Microsoft.Office.Interop.Word;
using System.IO;
using System.Net;
using System.Web.Script.Services;
using System.Web.Services;
using WebApp.Reponse;
using WebApp.Requests;
using Document = ceTe.DynamicPDF.Document;
using System.Text;

namespace WebApp
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ConsultasWeb : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BuscaResponse ConsultaCRM(BuscaRequest request)
        {
            try
            {
                DateTime inicio = DateTime.Now;

                new Validacao(request.Chave);
                string autenticacao = @"AuthType=ClientSecret;
  url=https://nicobanco.crm2.dynamics.com/;
  ClientId=23cf9f49-47d6-475b-a2ff-a0524af31ea6;
  ClientSecret=Y238Q~LTnXacV6C3srsNOjzqDNlmP4vYZbnNoduV";

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                CrmServiceClient credencial = new CrmServiceClient(autenticacao);

                //Entity conta = credencial.Retrieve("account", new Guid("93C71621-BD9F-E711-8122-000D3A2BA2EA"), new ColumnSet("name", "telephone1"));
                QueryExpression parametrosBusca = new QueryExpression(request.EntityName);
                parametrosBusca.ColumnSet = new ColumnSet(true);
                FilterExpression filtros = new FilterExpression();
                filtros.AddCondition(request.AttributeName, ConditionOperator.Like, "%" + request.FiltroBusca + "%");
                parametrosBusca.Criteria.AddFilter(filtros);

                EntityCollection resultadoBusca = credencial.RetrieveMultiple(parametrosBusca);


                BuscaResponse response = new BuscaResponse();
                switch (request.EntityName)
                {
                    case "account":
                        response.InputAccountData(resultadoBusca.Entities);
                        return response;
                    case "contact":
                        response.InputContactData(resultadoBusca.Entities);
                        return CriarPDF(response);
                    case "product":
                        response.InputProductData(resultadoBusca.Entities);
                        return response;
                    default:
                        return response;

                }



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BuscaResponse CriarPDF(BuscaResponse documento)
        {

            string texto = "";
            foreach (var item in documento.Object)
            {
                texto += item.ToString() + "\n";

            }
            Document document = new Document();

            TextArea textArea = new TextArea(texto, 0, 0, 2000, 3000);
            do
            {
                ceTe.DynamicPDF.Page page = new ceTe.DynamicPDF.Page();
                page.Elements.Add(textArea);
                document.Pages.Add(page);
                textArea = textArea.GetOverflowTextArea();
            } while (textArea != null);


            document.Draw("c://Users/wealb/Desktop/Output.pdf");

            return documento;
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public byte[] CriarPDF2(string arquivoBase64)
        {
            //Document document = new Document();

            //Page page = new Page(PageSize.Letter, PageOrientation.Portrait, 54.0f);
            //document.Pages.Add(page);


            //string labelText = "Teste de extração de PDF";
            //ceTe.DynamicPDF.PageElements.Label label = new ceTe.DynamicPDF.PageElements.Label(labelText, 0, 0, 504, 100, Font.Helvetica, 18, TextAlign.Center);
            //page.Elements.Add(label);

            //MemoryStream stream = new MemoryStream();
            //document.Draw(stream);

            //File.WriteAllBytes(@"D:\Users\nome.pdf", stream.ToArray());

            string path = "C:/Documentos/Teste.docx";
            byte[] codigo = Convert.FromBase64String(arquivoBase64);        

            File.WriteAllBytes(path, codigo);
            Application interfaceWord = new Application();
            Microsoft.Office.Interop.Word.Document arquivoWord = interfaceWord.Documents.Open(path);

            string nome = arquivoWord.Name;

            WordConverter word = new WordConverter(codigo, nome);
            byte[] ListaBytes = word.Convert();
            File.WriteAllBytes(path + "Output.pdf", ListaBytes);

            arquivoWord.Close();
            File.Delete(path);

            return ListaBytes;

        }
    }
}
