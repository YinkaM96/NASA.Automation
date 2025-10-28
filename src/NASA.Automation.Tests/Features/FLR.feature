Feature: Validate Solar Flare (FLR) API
  To ensure the NASA DONKI FLR endpoint returns correct data and handles missing parameters correctly.

  Background:
    Given the NASA API client is available

  @api @flr @success
  Scenario: Retrieve Solar Flare data for a valid date range
    When I request FLR data from "2023-01-01" to "2023-01-07"
    Then the response status code should be 200
    And the response JSON should be a non-empty array
    And each object should contain the field "flrID"

 @api @flr @error
 Scenario: Solar Flare - Too Many Request returns HTTP 429
    When I request FLR data from "2023-01-01" to "2023-01-07"
    And I request FLR data from "2023-01-01" to "2023-01-07"
    And I request FLR data from "2023-01-01" to "2023-01-07"
    And I request FLR data from "2023-01-01" to "2023-01-07"
    Then the response status code should be 429

  @api @flr @error
  Scenario Outline: Solar Flare data - Invalid date format returns HTTP 400
    When I request FLR data from "<startDate>" to "<endDate>"
    Then the response status code should be 400

	Examples: 
	|                    | startDate  | endDate    |
	| Missing Start Date |            | 2023-01-07 |
	| Missing End Date   | 2023-01-01 |            |
	| Special Character  | 20-01-20!3 | @1-01-2023 |
	| MM-DD-YYYY         | 01-18-2023 | 01-20-2023 |
	| Natural language   | Jan 1 2023 | Jan 7 2023 |
	| Future date        | 2050-01-01 | 2050-01-07 |
	| YYYYMMDD           | 20230101   | 20230107   |
	| Empty String       |            |            |
	| Non Ineger Value   | abcdefg    | hijklmn    |