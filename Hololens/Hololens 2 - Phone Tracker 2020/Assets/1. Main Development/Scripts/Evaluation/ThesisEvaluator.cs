using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;

namespace UOPHololens.Evaluation
{
    public class ThesisEvaluator : MonoBehaviour
    {
        public enum State { Idle, Loading, Saving, Evaluating }

        public BaseTester evaluationTest;

        internal SimpleFirebaseClient client;
        internal TMP_InputField usernameField;
        internal TMP_InputField ageField;
        internal TMP_Text targetsLeftCounter;
        internal TMP_Text basicTestInformation;
        internal SelectableTargetsProvider targetsProvider;
        internal EvaluationPlayer player;
        internal string path = "testers";

        private string username;
        State state = State.Idle;

        private void Awake()
        {
            usernameField.onSubmit.AddListener((username) =>
            {
                state = State.Loading;
                this.username = username;
                client.Get<EvaluationPlayer>(getPath(username), (player, valid) =>
                {
                    this.username = username;
                    if (!valid)
                        this.player = new EvaluationPlayer() { username = username };
                    else
                    {
                        this.player = player;
                        ageField.text = player.age.ToString();
                    }
                    state = State.Idle;
                });
            });     
        }
        public State CurrentState => state;
        public void Play()
        {
            if (Protect())
                return;
            state = State.Evaluating;
            evaluationTest.evaluator = this;
            StartCoroutine(evaluationTest.StartTest());
        }

        public void Save(Action<string, bool> onResult = null)
        {
            if (player == null)
                return;
            if (int.TryParse(ageField.text, out int age));
                player.age = age;
            client.Save(getPath(username), player, (data, valid) =>
            {
                Debug.Log(valid);
                onResult?.Invoke(data, valid);
            });
        }
        private bool Protect() => state != State.Idle || !enabled || evaluationTest == null;
        private string getPath(string username) => $"{path}/{username}.json";
    }
}
