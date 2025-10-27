Feature: Validate Coronal Mass Ejection (CME) API
  To verify that the NASA DONKI CME endpoint returns valid data and handles invalid inputs gracefully.

  Background:
    Given the NASA API client is available

  @api @cme @success
  Scenario: Retrieve CME data for a valid date range
    When I request CME data from "2023-01-01" to "2023-01-07"
    Then the response status code should be 200
    And the response JSON should be a non-empty array
    And each object should contain the field "activityID"

  @api @cme @error
  Scenario: Invalid date format returns HTTP 400
    When I request CME data from "20/01/2023" to "2023-01-07"
    Then the response status code should be 400
