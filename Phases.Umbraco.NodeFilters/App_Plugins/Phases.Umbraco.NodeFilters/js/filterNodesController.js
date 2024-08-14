angular.module("umbraco").controller("FilterNodesController", function ($scope, filterNodesResource, $http, $timeout) {
    var vm = this;
    vm.categories = [];
    vm.subcategories = [];
    vm.contentTypes = [];
    vm.values = [];
    vm.filteredNodes = [];
    vm.isApplyButtonDisabled = true;
    vm.isAddFilterButtonDisabled = false;
    filterNodesResource.get().then(function (response) {
        vm.filterSections.categories = response.data.categories;
        vm.filterSections.contentTypes = response.data.categories;

    });

    $scope.getSubcategories = function (category) {
        if (!category) return [];

        if (!$scope.subcategories[category]) {
            // Make an HTTP request to get subcategories based on the selected category
            $http.get('/umbraco/backoffice/filternodes/FilterNodesApi/GetSubcategories?category=' + category)
                .then(function (response) {
                    $scope.subcategories = response.data;
                });
        }

        return $scope.subcategories[category] || [];
    };

    $("#content-types").select2();
    $("#properties").select2();
    $("#values").select2();
    vm.updateSubcategories = function (index) {
        vm.checkDropdowns();
        var selectedCategoryId = vm.filterSections[index].category;

        if (selectedCategoryId >= 0 && selectedCategoryId != null) {
            $http.get('/umbraco/backoffice/filternodes/FilterNodesApi/GetSubcategories?category=' + selectedCategoryId)
                .then(function (response) {
                    vm.filterSections[index].subcategories = response.data.data;
                })
                .catch(function (error) {
                    console.error('Error fetching subcategories:', error);
                });
        } else {
            vm.filterSections[index].subcategories = []; // Clear subcategories if no category selected
        }
    };

    vm.updateValues = function (index) {
        vm.checkDropdowns();
        var selectedSubCategory = vm.filterSections[index].subcategories.filter(function (subCategory) {
            return subCategory.alias === vm.filterSections[index].subcategory
        });
        if (selectedSubCategory != null && selectedSubCategory.length > 0) {
            var selectedSubCategoryId = selectedSubCategory[0].id;


            $http.get('/umbraco/backoffice/filternodes/FilterNodesApi/GetValuesFromProperty?property=' + selectedSubCategoryId)
                .then(function (response) {
                    vm.filterSections[index].values = response.data.data;
                })
                .catch(function (error) {
                    console.error('Error fetching subcategories:', error);
                });
        }
        else {
            vm.filterSections[index].values = []; // Clear subcategories if no category selected
        }
    };

    vm.applyFilters = function () {
        $('#loader').show(); // Show the loader
        $('#result-section').hide(); // Hide the DataTable
        var filteredCategories = [];
        var filteredSubCategories = [];
        var filteredValues = [];
        var filteredStartDateValues = [];
        var filteredEndDateValues = [];
        // var allValuesForFilter = [];
        var filteredDataList = [];
        angular.forEach(vm.filterSections, function (filterSection) {

            if (vm.filterSections.categories !== undefined && vm.filterSections.categories !== null) {
                var filteredCategory = vm.filterSections.categories.filter(function (category) {
                    return category.id === filterSection.category;
                });

                if (filteredCategory.length > 0) {
                    filteredCategories.push(filteredCategory);
                }
            }

            if (filterSection.subcategories !== undefined && filterSection.subcategories !== null) {
                var filteredSubCategory = filterSection.subcategories.filter(function (subCategory) {
                    return subCategory.alias === filterSection.subcategory;
                });

                if (filteredSubCategory.length > 0) {
                    filteredSubCategories.push(filteredSubCategory);
                }
            }

            if (filterSection.values !== undefined && filterSection.values !== null) {
                var filteredValue = filterSection.values.filter(function (value) {
                    return value.id === filterSection.value;
                });

                if (filteredValue.length > 0) {
                    filteredValues.push(filteredValue);
                }
            }

            if (filterSection.startDate !== undefined && filterSection.startDate !== null) {
                var filteredDate = filterSection.startDate.toIsoDateString();


                filteredStartDateValues.push(filteredDate);

            }

            if (filterSection.endDate !== undefined && filterSection.endDate !== null) {
                var filteredEndDate = filterSection.endDate.toIsoDateString();


                filteredEndDateValues.push(filteredEndDate);

            }

            if (vm.filterSections.contentTypes !== undefined && vm.filterSections.contentTypes !== null) {
                var filteredContentType = vm.filterSections.contentTypes.filter(function (contentType) {
                    return contentType.id === filterSection.contentType;
                });

            }

            filteredDataList.push({
                filteredCategory: filteredCategory !== undefined && filteredCategory.length > 0 ? filteredCategory[0].alias : null,
                filteredSubCategory: filteredSubCategory !== undefined && filteredSubCategory.length > 0 ? filteredSubCategory[0].alias : null,
                filteredValue: filteredValue !== undefined && filteredValue.length > 0 ? filteredValue[0].name : null,
                filteredDate: filteredDate,
                filteredEndDate: filteredEndDate,
                filteredMainCategory: filterSection.mainCategory,
                filteredContentType: filteredContentType !== undefined && filteredContentType.length > 0 ? filteredContentType[0].id : null
            });
        });

        filterNodesResource.filterNodes(filteredDataList).then(function (response) {
            vm.filteredNodes = response.data.data;
            // Check if the DataTable instance already exists
            var dataTable = $('#dataTable').DataTable();
            if ($.fn.DataTable.isDataTable('#dataTable')) {
                // Clear and destroy the existing DataTable
                dataTable.clear().destroy();
            }
            // Initialize the DataTable
            $(document).ready(function () {
                $('#dataTable').DataTable();
            });
            setTimeout(function () {
                $('#loader').hide(); // Hide the loader after a delay
                $('#result-section').show(); // Show the DataTable
            }, 500); // Adjust the time (in milliseconds) as needed
        });
    };

    vm.onMainCategoryChange = function (index) {
        var filterSection = vm.filterSections[index];
        // Reset the model values
        filterSection.category = null;
        filterSection.subcategory = null;
        filterSection.value = null;

        // Clear the options for properties and values
        filterSection.subcategories = [];
        filterSection.values = [];

        // Reinitialize Select2
        $timeout(function () {
            $("#content-types").select2();
            $("#properties").select2();
            $("#values").select2();
        });

        // Clear date inputs
        filterSection.startDate = null;
        filterSection.endDate = null;


        // Recheck dropdowns to update button states
        vm.checkDropdowns();
    };

    vm.checkDropdownsV1 = function () {
        // Check if all three dropdowns have empty values
        if (vm.filterSections.length > 0) {
            for (var i = 0; i < vm.filterSections.length; i++) {
                var filterSection = vm.filterSections[i];

                if (filterSection.category === undefined || filterSection.subcategory === undefined
                    || filterSection.value === undefined || filterSection.category === null || filterSection.subcategory === null || filterSection.value === null) {
                    vm.isApplyButtonDisabled = true;
                    return; // One of the dropdowns is empty, so the button should be disabled
                }

            }
        }
        else {
            vm.isApplyButtonDisabled = true;
            return;
        }
        vm.isApplyButtonDisabled = false;
    };

    vm.shouldHideAddButton = function () {
        return this.filterSections.some(section => section.mainCategory === 'createdDate' || section.mainCategory === 'updatedDate');
    };

    vm.checkDropdowns = function () {
        // Check if there are any filter sections
        if (vm.filterSections.length > 0) {
            vm.isAddFilterButtonDisabled = true;
            for (var i = 0; i < vm.filterSections.length; i++) {
                var filterSection = vm.filterSections[i];
                // Check for the 'createdDate' category
                if (filterSection.mainCategory === 'createdDate' || filterSection.mainCategory === 'updatedDate') {
                    // Ensure both startDate and endDate are defined and not null
                    if (filterSection.startDate === undefined || filterSection.endDate === undefined ||
                        filterSection.startDate === null || filterSection.endDate === null ||
                        filterSection.startDate === '' || filterSection.endDate === '') {
                        vm.isApplyButtonDisabled = true;

                        return; // Start or End Date is empty, so disable the apply button
                    }
                } else {
                    // Check other categories for undefined or null values
                    /*
                    if (filterSection.category === undefined || filterSection.subcategory === undefined ||
                        filterSection.value === undefined || filterSection.category === null ||
                        filterSection.subcategory === null || filterSection.value === null) {
                        vm.isApplyButtonDisabled = true;
                        vm.isAddFilterButtonDisabled = true;
                        return; // One of the dropdowns is empty, so the button should be disabled
                    }
                    */
                    if (filterSection.category === undefined || filterSection.category === null) {
                        vm.isApplyButtonDisabled = true;
                        //vm.isAddFilterButtonDisabled = true;
                        return; // One of the dropdowns is empty, so the button should be disabled
                    }
                }
            }
        } else {
            vm.isApplyButtonDisabled = true;
            vm.isAddFilterButtonDisabled = false;
            return; // No filter sections added, so disable the apply button
        }
        // If all validations pass, enable the apply button
        vm.isApplyButtonDisabled = false;
        //vm.isAddFilterButtonDisabled = false;
    };

    vm.filterSections = [];

    vm.addFilterSection = function () {
        vm.filterSections.push({});
        vm.checkDropdowns();
    };

    vm.removeFilterSection = function (index) {

        vm.filterSections.splice(index, 1);
        vm.checkDropdowns();
        vm.clearDataTable();
    };

    vm.clearDataTable = function () {
        var dataTable = $('#dataTable').DataTable();
        if ($.fn.DataTable.isDataTable('#dataTable')) {
            // Clear and destroy the existing DataTable
            dataTable.clear().destroy();
        }
    };
}).directive('selectTwo', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            $timeout(function () {
                $(element).select2();
            });

            // Optional: Watch for model changes if you want to programmatically update Select2 when your model updates
            scope.$watch(attrs.ngModel, function () {
                $timeout(function () {
                    $(element).trigger('change');
                });
            });

            // Cleanup to prevent memory leak
            scope.$on('$destroy', function () {
                $(element).select2('destroy');
            });
        }
    };
});