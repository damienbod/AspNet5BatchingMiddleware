(function () {
	'use strict';

	function TestClassTwoService($http, $log) {

		$log.info("TestClassTwoService called");

		var gelAll = function () {
			$log.info("TestClassTwoController getAll called");
			return $http.get("/api/TestClassTwo")
			.then(function (response) {
				return response.data;
			});
		}

		return {
			getAll: getAll
		}
	}

	var module = angular.module('myapp');

	module.factory("TestClassTwoService",
		[
			"$http",
			"$log",
			TestClassTwoService
		]
	);
})();