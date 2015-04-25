(function () {
	'use strict';

	function TestClassTwoService($http, $log) {

		$log.info("TestClassTwoService called");

		var getAnigetAllmals = function () {
			$log.info("TestClassTwoController getAll called");
			return $http.get("/api/TestClassTwoController")
			.then(function (response) {
				return response.data;
			});
		}

		return {
			getAll: GetAll
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