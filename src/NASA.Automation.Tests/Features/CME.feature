Feature: Validate Coronal Mass Ejection (CME) API
  To verify that the NASA DONKI CME endpoint returns valid data and handles invalid inputs gracefully.

  Background:
    Given the NASA API client is available

 @API @CME @Success
  Scenario: Retrieve CME data for a valid date range
    When I request CME data from "2023-01-01" to "2023-01-07"
    Then the response status code should be 200
    And the response JSON should be a non-empty array
    And each object should contain the field "activityID"

@API @CME @Success
  Scenario: CME - Too Many Request returns HTTP 429
    When I request CME data from "2023-01-01" to "2023-01-07"
    And I request CME data from "2023-01-01" to "2023-01-07"
    And I request CME data from "2023-01-01" to "2023-01-07"
    And I request CME data from "2023-01-01" to "2023-01-07"
    And I request CME data from "2023-01-01" to "2023-01-07"
    Then the response status code should be 429

 @API @CME @Error
  Scenario Outline: CME - Invalid date format returns HTTP 400
    When I request CME data from "<startDate>" to "<endDate>"
    Then the response status code should be 400

Examples: 
	| Test              | startDate  | endDate    |
	| Special Character | 20-01-20!3 | @1-01-2023 |
	| MM-DD-YYYY        | 01-18-2023 | 01-20-2023 |
	| Natural language  | Jan 1 2023 | Jan 7 2023 |
	| Future date       | 2050-01-01 | 2050-01-07 |
	| YYYYMMDD          | 20230101   | 20230107   |
	| Empty String      |            |            |
	| Non Ineger Value  | abcdefg    | hijklmn    |

