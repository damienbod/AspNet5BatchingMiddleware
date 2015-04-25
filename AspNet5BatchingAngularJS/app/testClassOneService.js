(function () {
	'use strict';

	function TestClassOneService($http, $log) {

		$log.info("TestClassOneService called");

		var getAll = function () {
			$log.info("TestClassOneController getAll called");
			return $http.get("/api/TestClassOne")
			.then(function (response) {
				return response.data;
			});
		}

		return {
			getAll: getAll
		}
	}

	var module = angular.module('myapp');

    module.factory("TestClassOneService",
		[
			"$http",
			"$log",
			TestClassOneService
		]
	);
})();