Feature: NASA Open API Sign-Up Page
  To verify that users can navigate to the NASA API portal and complete the sign-up process successfully.

  Background: 
      Given I navigate to the NASA API home page

  @UI @Signup @Success
  Scenario: Complete the sign-up flow with a dummy email
    When I fill the registration form with:
        | FirstName | LastName | Email                    | Reason                     |
        | Test      | User     | testuser{guid}@gmail.com | Automated NASA API testing |
    And I submit the registration form
    Then I should see a confirmation or success message

  @UI @Signup @Success
  Scenario: Duplicate email is handled (if server-side enforced)
    When I fill the registration form with:
        | FirstName | LastName | Email                    | Reason                     |
        | Test      | User     | testuser{guid}@email.com | Automated NASA API testing |
    And I submit the registration form
    Then I should see a confirmation or success message
    Given I navigate to the NASA API home page
    When I fill the registration form with:
        | FirstName | LastName | Email                    | Reason                     |
        | Test      | User     | testuser{guid}@email.com | Automated NASA API testing |
    And I submit the registration form
    Then I should see a confirmation or success message

 @UI @Signup @Error
  Scenario: Complete the sign-up flow with a invalid email
    When I fill the registration form with:
        | FirstName | LastName | Email                | Reason                     |
        | Test      | User     | testuser@example.com | Automated NASA API testing |
    And I submit the registration form
    Then I should see a warning dialog for restricted email

  @UI @Signup @Error
  Scenario Outline: Complete the sign-up flow with a invalid input
    When I fill the registration form with:
        | FirstName   | LastName   | Email   | Reason                     |
        | <FirstName> | <LastName> | <Email> | Automated NASA API testing |
    And I submit the registration form
    Then I should see the correct warning message for "<Test>"

Examples: 
    | Test                    | FirstName | LastName | Email                 |
    | Invalid Email Character | Test      | User     | testuser@example@.com |
    | Empty First Name        |           | User     | testuser@example.com  |
    | Empty Last Name         | Test      |          | testuser@example.com  |
    | Empty Email             | Test      | User     |                       |


