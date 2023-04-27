namespace homework425
{
    internal class Program
    {// 먼저 Dictionaet에 키값과 데이터값을 넣어야하므로 일반화로 Tkey와 TValue를 설정 그리고 Tkey같은 나중에 해시테이블 인덱스값을
     // 나타내므로 개방주소법을 이용할려면 비교할수 있어야하므로 IEquatable 사용
        internal class Dictionary<Tkey, TValue> where Tkey : IEquatable<Tkey>
        {
            private const int DefultCapacity = 1000;            // 해시 테이블 크기크게가져서 용량을 포기하고
                                                                // 탐색에 의미를 둔다고해서 1000으로 설정

            public struct Entry                                 // 해시테이블 설정
            {                                                   // key, value 설정
                public enum State { None, Using, Delected }             // 상태는 개방주소법을 이용해야하므로 해시테이블의 상태를 이용할
                                                                        //예정이므로 상태 설정
                public Tkey key;
                public TValue value;
                public State state;
                public int hashCode;
            }

            private Entry[] table; // 해시테이블 설정

            public Dictionary()
            {
                table = new Entry[DefultCapacity];          //생성자 설정
            }


            public void Add(Tkey key, TValue value)
            {
                //일단 key값 해싱
                int index = Math.Abs((key.GetHashCode() % table.Length)); // index 음수일수도있으므로 abs사용하고 
                                                                          //% table.Length 이유는 해싱했는데
                                                                          //table.Length값을 넘어갈수도있어서 사용
                                                                          // 해시테이블에 해시값을 넣었을때 상태가 using이 될떄까지 반복
                while (table[index].state == Entry.State.Using)
                {
                    //일단 값 겹치면 에러
                    if (key.Equals(table[index].key))      //아까 맨위에서 Tkey같은 서로 비교가능하게 해서 Equals사용?
                        throw new ArgumentException();
                }
                //값이 안겹칠경우 
                // 해시테이블의 해시값위치의 인덱스에 key랑 value넣는다.
                // 그리고 using으로 설정
                table[index].hashCode = key.GetHashCode();              //해시테이블에 굳이 hasCoode까지 설정해주는 이유는
                                                                        // 충돌을 피할때 index가 해싱을 통한 해시값이랑 달라질수있으므로
                                                                        // 따로 hashCode를 넣어둔다.
                table[index].key = key;
                table[index].value = value;
                table[index].state = Entry.State.Using;
            }

            public TValue this[Tkey key]            //key에해당하는 Vlaue값 찾기
            {
                get
                {       // key에 대한 해시값 index로 설정하고
                    int index = Math.Abs(key.GetHashCode() % table.Length);
                    // 초기 해시값에 해당하는 해시테이블부터 끝까지 훓었을때까지
                    // 
                    while (index < table.Length)
                    {       // 만약 해시테이블 상태가 using이면 key값과 해시테이블의 key같과 비교하여 같으면 해당하는 테이블에 
                            // 저장된 value값을 리턴하고 아니면 index를 늘리고 cotinue한다.
                            // 만일 끝까지 해서 없으면 예외처리
                        if (table[index].state == Entry.State.Using)
                        {
                            if (key.Equals(table[index].key))
                            {
                                return table[index].value;
                            }
                        }
                        //인덱스 늘리면서 반복
                        else
                            index = ++index % table.Length;
                        continue;
                    }
                    throw new InvalidOperationException();              //없으면 예외처리
                }
                set
                {   //위에 get 방식과 동일하게 진행하여 만약에 해시테이블의 key값과 찾는 key같이 같으면 value값을 덮는다.
                    int index = Math.Abs(key.GetHashCode() % table.Length);

                    while (index < table.Length)
                    {
                        if (table[index].state == Entry.State.Using)
                        {
                            if (key.Equals(table[index].key))
                            {
                                table[index].value = value;
                            }
                        }
                        //인덱스 늘리면서 반복
                        else
                            index = ++index % table.Length;
                        continue;
                    }
                    throw new InvalidOperationException();// 없으면 예외처리

                }
            }

            public bool Remove(Tkey key)
            {   //먼저 key값 해싱
                int index = Math.Abs((key.GetHashCode() % table.Length));
                // 일단찾아야하므로 반복문을 사용하여 index값을 늘린다.
                while (index < table.Length)                  //일단 전부다 훓었을때
                {
                    if (table[index].state == Entry.State.None)
                        break;
                    if (table[index].state == Entry.State.Using)
                    {
                        if (key.Equals(table[index].key))       // 만일같다면 상태를 삭제로 바구고 true 반환
                        {
                            table[index].state = Entry.State.Delected;
                            return true;
                        }
                    }
                    else //  Entry.State.None이랑 
                         //인덱스 늘리면서 반복
                        index = ++index % table.Length;
                    continue;
                }
                return false;

            }
        }

   

    /*2.해싱이란 다양한 길이를 가진 데이터(키)를 고정된 길이를 가진 데이터(해시값)로 바꾸어 
     * 배열의 형태를 가지는 해시테이블의 특정인덱스(해시값이 나타내는 인덱스)위치에 키값에 해당하는 데이터를 넣는다.
     *  해시함수는 해싱과정에서 키를 해시값으로 바꿔주는 함수이다.
     *  해싱함수의 효율은 먼저 자료를 찾는데 빨라야하고, 해시테이블에 값을 저장하는형태이므로 해싱과정을통해 
     *  나온 해시값이 덜겹치게끔 설정해야한다. 마지막으로 해시테이블의 크기가 클수록 효율이 좋다.
     *  
     *  3. 해싱도 결국엔 키값은 무한에 가깝고 메모리에 해당할수 있는 해시테이블의 크기는 한정적이기때문에
     *  해싱을 하다가 해시값이 겹칠수도있다.이런걸 충돌이라하며 충돌의 해결방안으로는 체이싱이랑 개방주소법이있다.
     *  먼저 체이싱은 동일한 해시값을 갖게될경우 해시테이블의 인덱스안에서 링크드리스트로 연결하는방식이다.
     *  단점으로는 c#에서는 가비지컬렉터에 부담을주는 링크드리스트 방법을 선호하지 않기때문에 주로 개방주소법을 사용한다.
     *  개방주소법은 해시값을 갖을경우 나중에 넣는해시값이 기존의 해시테이블이 사용중인걸 파악한후 다음해시테이블의 인덱스로 가서
     *  비어있을경우 넣는방법이다.
     *
     *
     */


    static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
