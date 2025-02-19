﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Plugin
{
    public class PreValidationProduct : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                var contexto = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                if (contexto.MessageName.ToLower() == "create" && contexto.Mode == (int)Enum.Mode.Synchronous && contexto.Stage == (int)Enum.Stage.PreValidation)
                {
                    var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    var service = serviceFactory.CreateOrganizationService(contexto.UserId);
                    Entity produto = contexto.InputParameters["Target"] as Entity;

                    // QueryExpression consultaUnit = new QueryExpression("uom");
                    // QueryExpression consultaGrupoUnit = new QueryExpression("uomschedule");
                    //var unidade = service.RetrieveMultiple(consultaUnit);
                    //produto.Attributes["defaultuomid"] = new EntityReference("uom", unidade.Entities.First<Entity>().Id);

                    //var grupoUnidade = service.RetrieveMultiple(consultaGrupoUnit);
                    //produto.Attributes["defaultuomscheduleid"] = new EntityReference("uomschedule", grupoUnidade.Entities.First<Entity>().Id);
                    //QueryExpression consultaListaPreco = new QueryExpression("pricelevel");



                    //var listaPreco = service.RetrieveMultiple(consultaListaPreco);
                    //produto.Attributes["pricelevelid"] = new EntityReference("pricelevel", listaPreco.Entities.First<Entity>().Id);


                }
                
            }
            catch (FaultException<OrganizationServiceFault> f)
            {
                throw new InvalidPluginExecutionException(string.Format("Erro: {0} Detalhe: {1}", f.Message, f.Detail.Message));
            }
            catch (InvalidPluginExecutionException i)
            {
                throw new InvalidPluginExecutionException(i.Message);
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message, e);
            }
        }

        public void ValidaCampos(Entity produto)
        {
            ListaString listaErros = new ListaString();

            if (!produto.Contains("name") || produto.Attributes["name"].ToString() == "")
            {
                listaErros.Add("Nome");
            }
            if (!produto.Contains("naru_custo"))
                listaErros.Add("Custo");

            if (!produto.Contains("naru_preco"))
                listaErros.Add("Valor Mensal");

            if (!produto.Contains("naru_precoinstalacao"))
                listaErros.Add("Valor Instalação");

            if (!produto.Contains("naru_setor"))
                listaErros.Add("Setor");
            
            if (listaErros.ListaErro.Count != 0)
                throw new Exception("Os Campos " + listaErros.ToString() + " obrigatórios não preenchidos");
            
        }
        
        
    }
    public class ListaString
    {
        public List<string> ListaErro { get; set; }

        public void Add(string erro)
        {
            ListaErro.Add(erro);
        } 
        public override string ToString()
        {
            string nome = "";
            foreach (var item in ListaErro)
            {
                nome += item +", ";
            }

            return nome;
        }
    }
}
