Feature: NASA Open API Sign-Up Page
  To verify that users can navigate to the NASA API portal and complete the sign-up process successfully.

  Background: 
      Given I navigate to the NASA API home page

  @UI @Signup
  Scenario: Complete the sign-up flow with a dummy email
    When I start the registration process
    And I fill the registration form with:
        | FirstName | LastName | Email                | Reason                     |
        | Test      | User     | testuser@example.com | Automated NASA API testing |
    And I submit the registration form
    Then I should see a confirmation or success message

  @UI @Signup
  Scenario Outline: Complete the sign-up flow with a invalid input
    When I start the registration process
    And I fill the registration form with:
        | FirstName   | LastName   | Email   | Reason                     |
        | <FirstName> | <LastName> | <Email> | Automated NASA API testing |
    And I submit the registration form
    Then I should see a confirmation or success message

Examples: 
    | Test             | FirstName | LastName | Email                 |
    | Invalid Email    | Test      | User     | testuser@example@.com |
    | Empty First Name |           | User     | testuser@example.com  |
    | Empty Last Name  | Test      |          | testuser@example.com  |
    | Empty Email      | Test      | User     |                       |

  @UI @Signup
  Scenario: Duplicate email is handled (if server-side enforced)
    When I start the registration process
    And I fill the registration form with:
        | FirstName | LastName | Email                | Reason                     |
        | Test      | User     | testuser@example.com | Automated NASA API testing |
    And I submit the registration form
    And I fill the registration form with:
        | FirstName | LastName | Email                | Reason                     |
        | Test      | User     | testuser@example.com | Automated NASA API testing |
    And I submit the registration form

