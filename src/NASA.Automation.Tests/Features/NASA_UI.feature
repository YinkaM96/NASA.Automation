Feature: NASA Open API Sign-Up Page
  To verify that users can navigate to the NASA API portal and complete the sign-up process successfully.

  @ui @signup
  Scenario: Complete the sign-up flow with a dummy email
    When I navigate to the NASA API home page
    And I start the registration process
    And I fill the registration form with:
        | FirstName | LastName | Email                | Reason                     |
        | Test      | User     | testuser@example.com | Automated NASA API testing |
    And I submit the registration form
    Then I should see a confirmation or success message
