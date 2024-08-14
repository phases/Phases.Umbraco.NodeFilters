angular.module("umbraco.resources")
    .factory("filterNodesResource", function ($q, $http, umbRequestHelper) {
        return {

            get: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get("/umbraco/backoffice/filternodes/FilterNodesApi/GetAllCategories"), "Failed to retrieve all Person data");
            },
            getSubCategories: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get("/umbraco/backoffice/filternodes/FilterNodesApi/GetSubCategories"), "Failed to retrieve all Person data");
            },
            filterNodes: function (vm) {
                return $http({
                    method: "POST",
                    url: "/umbraco/backoffice/filternodes/FilterNodesApi/FilterNodes",
                    data: JSON.stringify(vm), // Strinify your object
                    headers: { 'Content-Type': 'application/json' }
                });
            }

        };

    });