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
  Scenario: Missing startDate returns HTTP 400
    When I request FLR data from "" to "2023-01-07"
    Then the response status code should be 400
