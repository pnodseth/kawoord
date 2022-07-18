/**
 * Enter here the user flows and custom policies for your B2C application
 * To learn more about user flows, visit: https://docs.microsoft.com/en-us/azure/active-directory-b2c/user-flow-overview
 * To learn more about custom policies, visit: https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-overview
 */
export const b2cPolicies = {
  names: {
    signUpSignIn: "B2C_1_susi_reset_v2",
    editProfile: "B2C_1_edit_profile_v2",
  },
  authorities: {
    signUpSignIn: {
      // authority: "https://fabrikamb2c.b2clogin.com/fabrikamb2c.onmicrosoft.com/B2C_1_susi_reset_v2",
      authority: "https://kawoord.b2clogin.com/kawoord.onmicrosoft.com/B2C_1_Kawoord_signup_signin",
    },
    forgotPassword: {
      authority: "https://kawoord.b2clogin.com/kawoord.onmicrosoft.com/B2C_1_Kawoord_rp",
    },
    editProfile: {
      authority: "https://kawoord.b2clogin.com/kawoord.onmicrosoft.com/B2C_1_Kawoord_ep",
    },
  },
  authorityDomain: "https://kawoord.b2clogin.com",
};
