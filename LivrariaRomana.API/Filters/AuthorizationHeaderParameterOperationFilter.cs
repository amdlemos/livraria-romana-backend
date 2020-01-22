//using Microsoft.AspNetCore.JsonPatch.Operations;
//using Microsoft.AspNetCore.Mvc.Authorization;
//using Microsoft.OpenApi.Models;
//using Ninject.Parameters;
//using Swashbuckle.AspNetCore.Swagger;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace LivrariaRomana.API.Filters
//{
//    public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
//    {
//        public void Apply(Operation operation, OperationFilterContext context)
//        {
//            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
//            var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is AuthorizeFilter);
//            var allowAnonymous = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);

//            if (isAuthorized && !allowAnonymous)
//            {
//                if (operation.Parameters == null)
//                    operation.Parameters = new List<IParameter>();

//                operation.Parameters.Add(new NonBodyParameter
//                {
//                    Name = "Authorization",
//                    In = "header",
//                    Description = "access token",
//                    Required = true,
//                    Type = "string"
//                });
//            }
//        }

//        public void Apply(OpenApiOperation operation, OperationFilterContext context)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
