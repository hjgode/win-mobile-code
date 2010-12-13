using System;
namespace AGR
{
	public class cVector3int
	{
/*		friend class ::cTestVector3int;
	
		cVector3int();
		cVector3int( const cVector3int& original );
		cVector3int( int x, int y, int z);
	
		// Overloaded operators
		cVector3int operator+=(const cVector3int &rhs);
		cVector3int& operator-=(const cVector3int &rhs); 
		cVector3int& operator*=(const int &rhs);
		cVector3int& operator/=(const int &rhs);
		const cVector3int operator+(const cVector3int &other) const;
		const cVector3int operator-(const cVector3int &other) const;
		const cVector3int operator*(const int &value) const;
		const cVector3int operator/(const int &value) const;
		bool operator==(const cVector3int &rhs) const;
		bool operator!=(const cVector3int &rhs) const;
		
	
		// other vector feature
		void copy( const cVector3int& original ); 
		bool copyTo( cVector3int* destination ) const; 
*/	
		private int _x;	/**< X Vector Component */
		public int x{
			get{return _x;}
			set{_x=value;}
		}
		private int _y;	/**< Y Vector Component */
		public int y{
			get{return _y;}
			set{_y=value;}
		}
		private int _z;	/**< Z Vector Component */
		public int z{
			get{return _z;}
			set{_z=value;}
		}

		/*! Constructor with arguments
		 */	
		public cVector3int(int x, int y, int z){
			this._x=x;
			this._y=y;
			this._z=z;
		}
		
		/*! Copy constructor
		 */	
		public cVector3int( cVector3int original) {
			//: x(original.x), y(original.y), z(original.z)
			this._x=original.x;
			this.y=original.y;
			this.z=original.z;
			
		}
		
		/*! Overloaded operator +=
		 */	
		public cVector3int Add( cVector3int rhs ) 
		{
			this._x += rhs.x;
			this._y += rhs.y;
			this._z += rhs.z;
			return this;
		}
		
	} // class cVector3int
	public struct iVector{
		public int x;
		public int y;
		public int z;
		public iVector(int a, int b, int c){
			this.x=a;
			this.y=b;
			this.z=c;
		}
		public static iVector operator + (iVector iV1, iVector iV2){
			return new iVector(iV1.x+iV2.x, iV1.y+iV2.y, iV1.z+iV2.z);
		}
		public static iVector operator - (iVector iV1, iVector iV2){
			return new iVector(iV1.x-iV2.x, iV1.y-iV2.y, iV1.z-iV2.z);
		}
		public static iVector operator * (iVector iV1, iVector iV2){
			return new iVector(iV1.x*iV2.x, iV1.y*iV2.y, iV1.z*iV2.z);
		}
		public static iVector operator / (iVector iV1, iVector iV2){
			return new iVector(iV1.x/iV2.x, iV1.y/iV2.y, iV1.z/iV2.z);
		}
		public static bool operator == (iVector iV1, iVector iV2){
			return (iV1.x==iV2.x && iV1.y==iV2.y && iV1.z==iV2.z);
		}
/*		public static bool operator Equals(iVector iV1, iVector iV2){
			return (iV1.x==iV2.x && iV1.y==iV2.y && iV1.z==iV2.z);
		}
*/
		public static bool operator != (iVector iV1, iVector iV2){
			return (iV1.x!=iV2.x || iV1.y != iV2.y || iV1.z!=iV2.z);
		}
		public iVector copy(iVector iv){
			return new iVector(iv.x,iv.y,iv.z);
		}
		public bool copyTo(iVector ivDest){
			ivDest.x=this.x;
			ivDest.y=this.y;
			ivDest.z=this.z;
			return true;
		}
		public float getSize(){
			return (float)System.Math.Sqrt(Math.Pow(this.x,2)+Math.Pow(this.y,2)+Math.Pow(this.z,2));
		}
	}
	public class cMovement{
		public iVector[] m_buffer;
		public int m_size;
		cMovement(){
			m_buffer=new iVector[0];
			m_size=0;
		}
		public cMovement(cMovement original){
			this.m_size=original.m_size;
			if(m_size>0){
				this.m_buffer=new iVector[m_size];
				for(int i=0;i<m_size;i++){
					m_buffer[i]=original.m_buffer[i];
				}
			}
			else
				m_buffer=new iVector[0];
		}
		public bool copy(cMovement original){
			this.m_size=original.m_size;
			if(m_size>0){
				this.m_buffer=new iVector[m_size];
				for(int i=0;i<m_size;i++){
					m_buffer[i]=original.m_buffer[i];
				}
			}
			else
				m_buffer=new iVector[0];
			return true;
		
		}
		public bool clear(){
			this.m_size=0;
			this.m_buffer=new iVector[0];
			return true;
		}
		public bool initialise(iVector[] inMove){
			if(this.m_buffer.Length>0)
				this.clear();
			if(inMove.Length==0)
				return false;
			// create buffer
			this.m_size=inMove.Length;
			this.m_buffer=new iVector[this.m_size];
			iVector[] iterBuffer=this.m_buffer;
			// copy data
			for(int i=0;i<m_size;i++){
				iterBuffer[i]=inMove[i];
			}
			return true;
		}
		/*! Initialisation using a buffer
		 * \return Success
		 */	
		bool initialise( int size,			/**< Size of the movement */
				iVector[] vectorArray )	/**< Array containing the initial value, set NULL if not needed */
		{
			// input checks
			if( 0 == size ) {
				return false; 
			}
		
			if( m_buffer.Length>0 ) {
				m_buffer=new iVector[0];
			}
		
			// create buffer
			m_size = size;
			m_buffer = new iVector[ m_size ];
			if( vectorArray.Length>0 ) {
				m_buffer=vectorArray;
				m_size=m_buffer.Length;
			}
		
			return true;
		}

		/*! Change the size of a movement
		 * \note OutputVector buffer will be created automatically if needed
		 * \note The function have better results if input movements are positive (see note in the code)
		 * \return Success
		 */	
		bool changeVectorSize( cMovement inputMovement,	/**< Input movement used as model */
					  int newSize,		/**< Size of the new vector */
					  cMovement ptOutputMovement )		/**< Output vector resized */
		{
			// input checks
			if( null == ptOutputMovement ||
				inputMovement == ptOutputMovement) 
			{
				return false;
			}
		
			if( ptOutputMovement.m_size != newSize )
			{
				// output buffer creation
				if( ptOutputMovement.m_buffer.Length==0 ) {
					ptOutputMovement.m_buffer=new iVector[0];
				}
				ptOutputMovement.m_size = newSize;
				ptOutputMovement.m_buffer = new iVector[ newSize ];
			}
		
		
			// Compute normalised vector
			for( int i=0; i<newSize; i++ )
			{
				// Start                  End
				// S-------X-------X------E		Movement
				//       A         B
				// S----------X-----------E		Normalised
				//         C = value to find
				// A B, closed value to C
				//
				// X = value(A)*(distanceBC/distanceAB) + value(B)*(distanceAC/distanceAB)
				// distanceAB = 1/ (movement vector size -1)
				// distanceSA = distanceAB* A index (same for SB)
				// distanceSC = ( 1/ (normalised vector size -1) ) * C index
				// distanceAC = distanceSC - distanceSA
				// distanceBC = distanceSB - distanceSC
		
				int indexC = i;
				float distanceAB = ( 1.0f/((float)inputMovement.m_buffer.Length -1.0f) );
				float distanceSC = ( 1.0f/((float)ptOutputMovement.m_buffer.Length-1.0f) ) * indexC;
				
				int indexA=0, indexB=0;
				for( int j=0; j<inputMovement.m_buffer.Length; j++)
				{
					if(  j*distanceAB == distanceSC )
					{
						indexA = j; 
						indexB = j;
						break;
					}
					if( j*distanceAB > distanceSC )
					{
						indexA = j-1; 
						indexB = j;
						break;
					}
				}
		
				float distanceSA = distanceAB * indexA;
				float distanceSB = distanceAB * indexB;
				float distanceAC = distanceSC - distanceSA;
				float distanceBC = distanceSB - distanceSC;
		
				if( indexA == indexB )
				{
					ptOutputMovement.m_buffer[i] = inputMovement.m_buffer[indexA];
				}
				else
				{
					// 0.5f added to have a better float to int cast
					// NOTE : this works ONLY when the float value is greater than 0
					// In the case of the accelerometers values as used in AGR, this is not an issue. 
					ptOutputMovement.m_buffer[i].x = (int)( 0.5f + inputMovement.m_buffer[indexA].x*(distanceBC/distanceAB) + 
												  inputMovement.m_buffer[indexB].x*(distanceAC/distanceAB) );
					ptOutputMovement.m_buffer[i].y = (int)( 0.5f + inputMovement.m_buffer[indexA].y*(distanceBC/distanceAB) + 
												  inputMovement.m_buffer[indexB].y*(distanceAC/distanceAB) );
					ptOutputMovement.m_buffer[i].z = (int)( 0.5f + inputMovement.m_buffer[indexA].z*(distanceBC/distanceAB) + 
												  inputMovement.m_buffer[indexB].z*(distanceAC/distanceAB) );
				}
			}
		
			return true;
		}
	
	}
}

