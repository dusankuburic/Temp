﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Temp.API.Helpers;

public static class Extensions
{
    public static void AddApplicationError(this HttpResponse response, string message) {
        response.Headers.Append("Application-Error", message);
        response.Headers.Append("Access-Control-Expose-Headers", "Application-Error");
        response.Headers.Append("Access-Control-Allow-Origin", "*");
    }

    public static void AddPagination(this HttpResponse response, int currentPage,
        int itemsPerPage, int totalItems, int totalPages) {
        var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
        var camelCaseFormatter = new JsonSerializerSettings();
        camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();

        response.Headers.Append("Pagination", JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
        response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
    }
}
